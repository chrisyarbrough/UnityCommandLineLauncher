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

			var version = ProjectVersionFile.Parse(projectDir);
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
}