class OpenCommand : BaseCommand<OpenSettings>
{
	protected override int ExecuteImpl(OpenSettings settings)
	{
		var searchPath = settings.SearchPath ?? PromptForRecentProject(settings.Favorite);

		searchPath = Path.GetFullPath(searchPath);

		if (!Directory.Exists(searchPath) && !File.Exists(searchPath))
		{
			WriteError($"'{searchPath}' does not exist.");
			return 1;
		}

		UnityVersion unityVersion = ProjectVersionFile.Parse(searchPath, out string filePath);
		Debug.WriteLine($"File: {filePath}\n{unityVersion}\nVersion: {unityVersion}");

		new UnityHub(settings.MutatingProcess)
			.EnsureEditorInstalled(unityVersion.Version, unityVersion.Changeset);

		var editorPath = UnityHub.GetEditorPath(unityVersion.Version);
		AnsiConsole.MarkupLine($"[dim]Editor: {editorPath}[/]");

		string projectDir = new FileInfo(filePath).Directory!.Parent!.FullName;
		string[] additionalArgs = context.Remaining.Raw.ToArray();
		var args = new List<string> { "-projectPath", projectDir };
		args.AddRange(additionalArgs);

		settings.MutatingProcess.Run(new ProcessStartInfo(fileName: editorPath, arguments: JoinQuoted(args)));

		if (settings.CodeEditor)
		{
			OpenSolutionFile(projectDir, settings.MutatingProcess);
		}

		// Unity doesn't report an exit code if the editor fails to open a project.
		// Instead, it prints error into the log. So, for now, we must assume it succeeded.
		return 0;
	}

	private static string JoinQuoted(List<string> args)
	{
		return string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a));
	}

	private static string PromptForRecentProject(bool favoritesOnly)
	{
		var recentProjects = UnityHub.GetRecentProjects(favoritesOnly).ToArray();

		if (recentProjects.Length == 0)
			throw new Exception("No projects found in Unity Hub.");

		return SelectionPrompt.Prompt(
			recentProjects,
			$"Select a {(favoritesOnly ? "favorite" : "recent")} project: ");
	}

	private static void OpenSolutionFile(string projectDir, IProcessRunner processRunner)
	{
		var path = Directory.GetFiles(projectDir, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

		if (path == null)
			return;

		AnsiConsole.MarkupLine($"[dim]Solution: {Path.GetFileName(path)}[/]");

		processRunner.Run(PlatformSupport.GetOpenFileProcess(path));
	}
}