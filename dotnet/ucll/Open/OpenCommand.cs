using System.Diagnostics;

class OpenCommand : BaseCommand<OpenSettings>
{
	protected override int ExecuteImpl(OpenSettings settings)
	{
		var searchPath = settings.SearchPath ?? PromptForRecentProject(settings.Favorites);

		if (string.IsNullOrEmpty(searchPath))
		{
			AnsiConsole.MarkupLine("No search path provided. Aborting.");
			return 0;
		}

		searchPath = Path.GetFullPath(searchPath);

		if (!Directory.Exists(searchPath) && !File.Exists(searchPath))
		{
			WriteError($"'{searchPath}' does not exist.");
			return 1;
		}

		UnityVersion unityVersion = ProjectVersionFile.Parse(searchPath, out string filePath);
		Debug.WriteLine($"File: {filePath}\n{unityVersion}\nVersion: {unityVersion}");

		new UnityHub(settings.MutatingProcess)
			.EnsureEditorInstalledAsync(unityVersion.Version, unityVersion.Changeset).Wait();

		var editorPath = UnityHub.GetEditorPath(unityVersion.Version);
		AnsiConsole.MarkupLine($"[dim]Editor: {editorPath}[/]");

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

		try
		{
			// This supports fuzzy matching and acronyms.
			return PromptWithFzf(recentProjects);
		}
		catch (Exception)
		{
			return AnsiConsole.Prompt(
				new SelectionPrompt<string>()
					.Title($"Select a [green]{(favoritesOnly ? "favorite" : "recent")} project[/]:")
					.EnableSearch()
					.SearchPlaceholderText("[dim](Type to search. Install 'fzf' for fuzzy matching.)[/]")
					.AddChoices(recentProjects));
		}
	}

	private static string PromptWithFzf(List<string> projects)
	{
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "fzf",
				Arguments = "--prompt=\"Select project: \" --height=40% --reverse --bind=change:first -i",
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
			},
		};

		process.Start();

		foreach (var project in projects)
			process.StandardInput.WriteLine(project);

		process.StandardInput.Close();

		string result = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();
		return result;
	}
}