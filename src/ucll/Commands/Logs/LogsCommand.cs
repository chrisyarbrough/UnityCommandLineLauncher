internal class LogsCommand(PlatformSupport platformSupport, UnityHub unityHub)
	: SearchPathCommand<LogsSettings>(unityHub)
{
	protected override int ExecuteImpl(LogsSettings settings)
	{
		string searchPath = ResolveSearchPath(settings.SearchPath, settings.Favorite);
		string projectPath = Project.FindProjectPath(searchPath);

		var paths = UnityLogPaths.Resolve(projectPath, UnityHub.GetProjectArgs(projectPath), platformSupport)
			.Where(p => settings.Type.Equals("all", StringComparison.OrdinalIgnoreCase)
				|| p.Type.Equals(settings.Type, StringComparison.OrdinalIgnoreCase))
			.ToList();

		foreach (var path in paths)
		{
			Console.WriteLine($"{path.Type}: {path.Path}");

			if (!settings.PathOnly)
				OpenLogPath(path.Path, settings.MutatingProcess);
		}

		return 0;
	}

	private void OpenLogPath(string logPath, IProcessRunner processRunner)
	{
		bool logFileExists = File.Exists(logPath);
		string? pathToOpen = logFileExists
			? logPath
			: Path.GetDirectoryName(logPath);

		if (pathToOpen == null || (!logFileExists && !Directory.Exists(pathToOpen)))
		{
			AnsiConsole.MarkupLine($"[yellow]Log path does not exist yet: {Markup.Escape(logPath)}[/]");
			return;
		}

		var process = processRunner.Run(platformSupport.OpenFile(pathToOpen));
		process.WaitForExit();
	}
}
