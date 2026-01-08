internal class VersionUsage
{
	public HashSet<string> Installed { get; }
	public HashSet<string> Used { get; }
	public HashSet<string> UsedNotInstalled => Used.Except(Installed).ToHashSet();
	public HashSet<string> InstalledNotUsed => Installed.Except(Used).ToHashSet();

	public VersionUsage(PlatformSupport platformSupport, UnityHub unityHub)
	{
		Installed = unityHub.ListInstalledEditors()
			.Select(info => info.Version)
			.ToHashSet();

		Used = FindUnityProjects(platformSupport)
			.Select(p => ProjectVersionFile.Parse(p, out string _).Version)
			.ToHashSet();
	}

	public static IEnumerable<string> FindUnityProjects(PlatformSupport platformSupport)
	{
		var startInfo = platformSupport.GetUnityProjectSearchProcess();
		startInfo.RedirectStandardOutput = true;
		var process = ProcessRunner.Default.Run(startInfo);
		process.WaitForExit();

		while (!process.StandardOutput.EndOfStream)
			yield return process.StandardOutput.ReadLine()!;
	}
}