internal class CreateCommand(UnityHub unityHub) : BaseCommand<CreateSettings>
{
	protected override int ExecuteImpl(CreateSettings settings)
	{
		ValidateProjectDoesNotExist(settings.ProjectPath);

		string version;
		if (settings.Version != null)
		{
			version = settings.Version;
		}
		else
		{
			AnsiConsole.MarkupLine("[dim]No version specified. Searching for available editors...[/]");
			var versions = unityHub.ListInstalledEditors().Select(editor => editor.Version);
			version = SelectionPrompt.Prompt(UnityVersion.SortNewestFirst(versions), "Select Unity version");
		}

		string editorPath = unityHub.GetEditorPath(version);
		AnsiConsole.MarkupLine($"[dim]Editor: {editorPath}[/]");

		AnsiConsole.MarkupLine($"[cyan]Creating Unity project...[/]");

		if (settings.Minimal)
		{
			string changeset = UnityReleaseApi.FetchChangesetAsync(version).Result;
			CreateManually(settings.ProjectPath, version, changeset);
		}
		else
		{
			var args = new List<string>
			{
				"-batchmode",
				"-quit",
				"-createProject",
				settings.ProjectPath,
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

		WriteSuccess($"Unity project created successfully.");
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