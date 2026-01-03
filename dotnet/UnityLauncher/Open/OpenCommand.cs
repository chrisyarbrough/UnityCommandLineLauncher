class OpenCommand : BaseCommand<OpenSettings>
{
	protected override int ExecuteImpl(OpenSettings settings)
	{
		var searchPath = settings.SearchPath ?? PromptForRecentProject(settings.Favorites);
		searchPath = Path.GetFullPath(searchPath);

		if (!Directory.Exists(searchPath))
		{
			WriteError($"Directory '{searchPath}' does not exist.");
			return 1;
		}

		UnityVersion unityVersion = ProjectVersionFile.Parse(searchPath, out string filePath);
		AnsiConsole.MarkupLine($"File: {filePath}");
		AnsiConsole.MarkupLine($"Version: {unityVersion}");

		new UnityHub(settings.MutatingProcess)
			.EnsureEditorInstalledAsync(unityVersion.Version, unityVersion.Changeset).Wait();

		var editorPath = UnityHub.GetEditorPath(unityVersion.Version);
		AnsiConsole.MarkupLine($"Editor: {editorPath}");

		string projectDir = new FileInfo(filePath).Directory!.Parent!.FullName;
		string[] additionalArgs = settings.UnityArgs ?? [];
		var args = new List<string> { "-projectPath", projectDir };
		args.AddRange(additionalArgs);

		settings.MutatingProcess.Run(
			fileName: editorPath,
			redirectOutput: false,
			args: string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)));

		// Unity doesn't report an exit code if the editor fails to open a project.
		// Instead, it prints error into the log. So, for now, we just assume it succeeded.
		return 0;
	}

	private static string PromptForRecentProject(bool favoritesOnly)
	{
		var recentProjects = UnityHub.GetRecentProjects(favoritesOnly).ToList();

		if (recentProjects.Count == 0)
			throw new Exception("No projects found in Unity Hub.");

		return AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title($"Select a [green]{(favoritesOnly ? "favorite" : "recent")} project[/]:")
				.EnableSearch()
				.AddChoices(recentProjects));
	}
}