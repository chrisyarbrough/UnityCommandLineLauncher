internal class CreateCommand(UnityHub unityHub) : BaseCommand<CreateSettings>
{
	protected override int ExecuteImpl(CreateSettings settings)
	{
		string projectPath = settings.ProjectPath;

		ValidateProjectDoesNotExist(projectPath);

		string editorPath = unityHub.GetEditorPath(settings.Version);
		AnsiConsole.MarkupLine($"[dim]Editor: {editorPath}[/]");

		AnsiConsole.MarkupLine($"[cyan]Creating Unity project at {projectPath}...[/]");

		if (settings.Minimal)
		{
			string changeset = UnityReleaseApi.FetchChangesetAsync(settings.Version).Result;
			CreateManually(projectPath, settings.Version, changeset);
		}
		else
		{
			var args = new List<string>
			{
				"-batchmode",
				"-quit",
				"-createProject",
				projectPath,
			};

			var process = settings.MutatingProcess.Run(
				new ProcessStartInfo(fileName: editorPath, arguments: ProcessRunner.JoinQuoted(args)));

			process.WaitForExit();

			if (process.ExitCode != 0)
			{
				WriteError($"Failed to create Unity project. Unity exited with code {process.ExitCode}");
				return 1;
			}
		}

		WriteSuccess($"Unity project created successfully at {projectPath}");
		return 0;
	}

	private static void CreateManually(string projectPath, string version, string changeset)
	{
		Directory.CreateDirectory(Path.Combine(projectPath, "Assets"));

		CreateFile(Path.Combine(projectPath, "ProjectSettings", "ProjectVersion.txt"),
			$"""
			 m_EditorVersion: {version}
			 m_EditorVersionWithRevision: {version} ({changeset})

			 """);

		CreateFile(Path.Combine(projectPath, "Packages", "manifest.json"),
			"""
			{
			  "dependencies": {
			  }
			}

			""");
	}

	private static void CreateFile(string path, string content)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		File.WriteAllText(path, content);
	}

	private static void ValidateProjectDoesNotExist(string projectPath)
	{
		if (!Directory.Exists(projectPath))
			return;

		if (Directory.GetFileSystemEntries(projectPath).Any(f => !Path.GetFileName(f).StartsWith('.')))
		{
			throw new UserException(
				$"Directory '{projectPath}' already exists and is not empty. " +
				"Choose a different location or use an empty directory.");
		}
	}
}