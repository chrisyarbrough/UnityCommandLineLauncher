class VersionUsage
{
	public HashSet<string> Installed { get; }
	public HashSet<string> Used { get; }
	public HashSet<string> UsedNotInstalled => Used.Except(Installed).ToHashSet();
	public HashSet<string> InstalledNotUsed => Installed.Except(Used).ToHashSet();

	public static VersionUsage PerformSearch() => new VersionUsage();

	private VersionUsage()
	{
		Installed = UnityHub.ListInstalledEditors()
			.Select(i => i.Version)
			.ToHashSet();

		Used = FindUnityProjects()
			.Select(p => ProjectVersionFile.Parse(p, out string _).Version)
			.ToHashSet();
	}

	public static IEnumerable<string> FindUnityProjects()
	{
		var startInfo = PlatformSupport.GetUnityProjectSearchProcess();
		startInfo.RedirectStandardOutput = true;
		var process = ProcessRunner.Default.Run(startInfo);
		process.WaitForExit();

		while (!process.StandardOutput.EndOfStream)
			yield return process.StandardOutput.ReadLine()!;
	}
}