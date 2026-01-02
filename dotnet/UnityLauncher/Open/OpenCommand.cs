class OpenCommand : BaseCommand<OpenSettings>
{
	protected override int ExecuteImpl(OpenSettings settings)
	{
		var projectDir = Path.GetFullPath(settings.SearchPath);

		if (!Directory.Exists(projectDir))
		{
			ConsoleHelper.WriteError($"Project directory '{settings.SearchPath}' is not a valid directory.");
			return 1;
		}

		var version = ProjectVersionFile.Parse(projectDir, out string filePath);
		AnsiConsole.MarkupLine($"[cyan]File: {filePath}[/]");
		string logString = $"Version: {version.Version}";
		if (version.Changeset != null)
			logString += $" ({version.Changeset})";
		AnsiConsole.MarkupLine(logString);

		UnityHub.EnsureEditorInstalledAsync(version.Version, version.Changeset).Wait();

		var editorPath = UnityHub.GetEditorPath(version.Version);
		AnsiConsole.MarkupLine($"Editor: {editorPath}");

		string[] additionalArgs = settings.UnityArgs ?? [];
		var args = new List<string> { "-projectPath", projectDir };
		args.AddRange(additionalArgs);

		ProcessHelper.Run(
			fileName: Path.Combine(editorPath, "Contents/MacOS/Unity"),
			redirectOutput: false,
			args: string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)));

		ConsoleHelper.WriteSuccess("Unity Editor launched successfully.");
		return 0;
	}
}