internal sealed class MacSupport : PlatformSupport
{
	public override ProcessStartInfo OpenFile(string filePath)
		=> new ProcessStartInfo("open", filePath);

	public override ProcessStartInfo OpenFileWithApp(string filePath, string applicationPath)
	{
		// Handle .app bundles.
		return new ProcessStartInfo("open", $"-a \"{applicationPath}\" \"{filePath}\"");
	}

	public override string RelativeEditorPathToExecutable => "Contents/MacOS/Unity";

	public override string UnityHubConfigDirectory => Path.Combine(UserHome, "Library/Application Support/UnityHub");

	public override ProcessStartInfo GetUnityProjectSearchProcess()
	{
		// Automatically indexed Spotlight search.
		return new ProcessStartInfo("bash",
			"-c \"mdfind 'kMDItemFSName == ProjectVersion.txt' | grep ProjectSettings/ProjectVersion.txt\"");
	}

	protected override string DefaultEditorPathTemplate =>
		"/Applications/Unity/Hub/Editor/{0}/Unity.app/Contents/MacOS/Unity";

	protected override string DefaultUnityHubPath => "/Applications/Unity Hub.app/Contents/MacOS/Unity Hub";

	public override string? GetUnityScriptingEditorPath()
	{
		var process = new ProcessRunner().Run(
			new ProcessStartInfo("defaults", "read com.unity3d.UnityEditor5.x kScriptsDefaultApp")
			{
				RedirectStandardOutput = true,
			});

		string output = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();

		return process.ExitCode == 0 ? output : null;
	}

	protected override string? FindUnityHub()
	{
		var process = new ProcessRunner().Run(
			new ProcessStartInfo("mdfind", "kMDItemCFBundleIdentifier == 'com.unity3d.unityhub'")
			{
				RedirectStandardOutput = true,
			});

		string output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
			return null;

		string[] lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		if (lines.Length == 0)
			return null;

		return Path.Combine(lines[0], "Contents", "MacOS", "Unity Hub");
	}
}