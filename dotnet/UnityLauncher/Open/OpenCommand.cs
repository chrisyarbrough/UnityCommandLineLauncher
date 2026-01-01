class OpenCommand : Command<OpenSettings>
{
	public override int Execute(CommandContext context, OpenSettings settings, CancellationToken cancellationToken)
	{
		try
		{
			var projectDir = Path.GetFullPath(settings.ProjectPath);

			if (!Directory.Exists(projectDir))
			{
				AnsiConsole.MarkupLine(
					$"[red]Error: Project directory '{settings.ProjectPath}' is not a valid directory[/]");
				return 1;
			}

			var versionFile = FindProjectVersionFile(projectDir);
			AnsiConsole.MarkupLine($"[cyan]File: {versionFile}[/]");

			var version = ProjectVersionFile.Parse(versionFile);
			AnsiConsole.MarkupLine($"[cyan]Version: {version.Version}[/]");

			UnityHub.EnsureEditorInstalledAsync(version.Version, version.Changeset)
				.Wait(cancellationToken);

			var editorPath = UnityHub.GetEditorPath(version.Version);
			if (editorPath == null)
			{
				AnsiConsole.MarkupLine($"[red]Error: Unity version {version.Version} is not installed[/]");
				return 1;
			}

			AnsiConsole.MarkupLine($"[cyan]Editor: {editorPath}[/]");

			string[] additionalArgs = settings.UnityArgs ?? [];
			var args = new List<string> { "-projectPath", projectDir };
			args.AddRange(additionalArgs);

			ProcessHelper.Run(Path.Combine(editorPath, "Contents/MacOS/Unity"), redirectOutput: false, args);

			AnsiConsole.MarkupLine("[green]Unity Editor launched successfully[/]");

			return 0;
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
			return 1;
		}
	}

	private static string FindProjectVersionFile(string searchDir)
	{
		var foundFiles = Directory.EnumerateFiles(
				searchDir, "ProjectVersion.txt", SearchOption.AllDirectories)
			.Where(file => Path.GetFileName(Path.GetDirectoryName(file)) == "ProjectSettings").ToArray();

		if (foundFiles.Length == 0)
			throw new Exception($"No ProjectSettings/ProjectVersion.txt found in {searchDir}");

		if (foundFiles.Length > 1)
		{
			throw new Exception(
				$"Found multiple ProjectVersion.txt files:\n{string.Join("\n", foundFiles)}\n" +
				"Please run in a directory with only one Unity project");
		}

		return foundFiles[0];
	}
}