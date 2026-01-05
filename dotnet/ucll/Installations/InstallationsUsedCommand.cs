class InstallationsUsedCommand : BaseCommand<InstallationsUsedSettings>
{
	protected override int ExecuteImpl(InstallationsUsedSettings settings)
	{
		var projectsUsingVersion = FindProjectsUsingVersion(settings.Version).ToList();
		foreach (string project in projectsUsingVersion)
		{
			Console.WriteLine(project);
		}
		return 0;
	}

	private static IEnumerable<string> FindProjectsUsingVersion(string version)
	{
		var startInfo = PlatformHelper.GetUnityProjectSearchProcess();
		startInfo.RedirectStandardOutput = true;
		var process = ProcessRunner.Default.Run(startInfo);
		process.WaitForExit();

		while (!process.StandardOutput.EndOfStream)
		{
			string projectPath = process.StandardOutput.ReadLine()!;
			var projectVersion = ProjectVersionFile.Parse(projectPath, out string _).Version;

			if (projectVersion == version)
			{
				yield return projectPath;
			}
		}
	}
}