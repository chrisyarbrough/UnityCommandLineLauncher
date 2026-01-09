internal class CreateCommand(UnityHub unityHub) : BaseCommand<CreateSettings>
{
	protected override int ExecuteImpl(CreateSettings settings)
	{
		string projectPath = settings.ProjectPath;

		ValidateProjectDoesNotExist(projectPath);

		// 3. Get editor path (throws UserException if not installed)
		string editorPath = unityHub.GetEditorPath(settings.Version);
		AnsiConsole.MarkupLine($"[dim]Editor: {editorPath}[/]");

		// 4. Create parent directory if needed
		string? parentDir = Path.GetDirectoryName(projectPath);
		if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
		{
			AnsiConsole.MarkupLine($"[dim]Creating parent directory: {parentDir}[/]");
			Directory.CreateDirectory(parentDir);
		}

		// 5. Build Unity command arguments
		var args = new List<string>
		{
			"-batchmode",
			"-quit",
			"-createProject",
			projectPath,
		};

		// 6. Add any additional user arguments
		string[] additionalArgs = Context.Remaining.Raw.ToArray();
		args.AddRange(additionalArgs);

		// 7. Execute Unity
		AnsiConsole.MarkupLine($"[cyan]Creating Unity project at {projectPath}...[/]");

		var process = settings.MutatingProcess.Run(
			new ProcessStartInfo(fileName: editorPath, arguments: JoinQuoted(args)));

		process.WaitForExit();

		// 8. Handle result
		if (process.ExitCode != 0)
		{
			WriteError($"Failed to create Unity project. Unity exited with code {process.ExitCode}");
			return 1;
		}

		WriteSuccess($"Unity project created successfully at {projectPath}");
		WriteSuccess($"Open with: ucll open \"{projectPath}\"");
		return 0;
	}

	private static void ValidateProjectDoesNotExist(string projectPath)
	{
		if (!Directory.Exists(projectPath))
			return;

		string projectSettingsDir = Path.Combine(projectPath, "ProjectSettings");
		if (Directory.Exists(projectSettingsDir))
		{
			throw new UserException(
				$"A Unity project already exists at '{projectPath}'. " +
				"Choose a different location or delete the existing project.");
		}

		if (Directory.GetFileSystemEntries(projectPath).Any(f => !Path.GetFileName(f).StartsWith('.')))
		{
			throw new UserException(
				$"Directory '{projectPath}' already exists and is not empty. " +
				"Choose a different location or use an empty directory.");
		}
	}

	private static string JoinQuoted(List<string> args)
	{
		return string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a));
	}
}