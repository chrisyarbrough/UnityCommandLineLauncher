class OpenCommand : BaseCommand<OpenSettings>
{
	protected override int ExecuteImpl(OpenSettings settings)
	{
		var searchPath = Path.GetFullPath(settings.SearchPath);

		if (!Directory.Exists(searchPath))
		{
			ConsoleHelper.WriteError($"Project directory '{settings.SearchPath}' is not a valid directory.");
			return 1;
		}

		var version = ProjectVersionFile.Parse(searchPath, out string filePath);
		AnsiConsole.MarkupLine($"[cyan]File: {filePath}[/]");
		string logString = $"Version: {version.Version}";
		if (version.Changeset != null)
			logString += $" ({version.Changeset})";
		AnsiConsole.MarkupLine(logString);

		UnityHub.EnsureEditorInstalledAsync(version.Version, version.Changeset).Wait();

		var editorPath = UnityHub.GetEditorPath(version.Version);
		AnsiConsole.MarkupLine($"Editor: {editorPath}");

		string projectDir = new FileInfo(filePath).Directory!.Parent!.FullName;
		string[] additionalArgs = settings.UnityArgs ?? [];
		var args = new List<string> { "-projectPath", projectDir };
		args.AddRange(additionalArgs);

		ProcessHelper.Run(
			fileName: Path.Combine(editorPath, "Contents/MacOS/Unity"),
			redirectOutput: false,
			args: string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)));

		// Sadly, Unity doesn't report an exit code if the editor fails to open a project.
		// Instead, it prints error into the log. So, for now, we just assume it succeeded.
		return 0;
	}
}