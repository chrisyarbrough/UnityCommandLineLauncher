sealed class MacSupport : PlatformSupport
{
	protected override string GetRelativeEditorPathToExecutableCore()
		=> "Contents/MacOS/Unity";

	protected override int GetInstallationLevelsToGoUp()
		=> 4;

	protected override string GetEditorPathPattern()
		=> "/Applications/Unity/Hub/Editor/{0}/Unity.app/Contents/MacOS/Unity";

	protected override string GetHubPathPattern()
		=> "/Applications/Unity Hub.app/Contents/MacOS/Unity Hub";

	protected override string GetConfigDirectoryPath()
		=> Path.Combine(UserHome, "Library/Application Support/UnityHub");

	protected override IEnumerable<string?> GetPlatformSpecificHubPaths()
	{
		yield return FindUnityHubPathMacOS();
	}

	private static string? FindUnityHubPathMacOS()
	{
		var process = new ProcessRunner().Run(
			new ProcessStartInfo("mdfind", "kMDItemCFBundleIdentifier == 'com.unity3d.unityhub'")
			{
				RedirectStandardOutput = true,
			});

		var output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
			return null;

		var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		if (lines.Length == 0)
			return null;

		return Path.Combine(lines[0], "Contents", "MacOS", "Unity Hub");
	}

	protected override ProcessStartInfo GetUnityProjectSearchProcessCore()
	{
		// Automatically indexed Spotlight search.
		return new ProcessStartInfo("bash",
			"-c \"mdfind 'kMDItemFSName == ProjectVersion.txt' | grep ProjectSettings/ProjectVersion.txt\"");
	}

	protected override ProcessStartInfo GetOpenFileProcessCore(string filePath)
		=> new ProcessStartInfo("open", filePath);

	protected override ProcessStartInfo GetOpenFileWithApplicationProcessCore(string applicationPath, string filePath)
	{
		// On macOS, use 'open -a' to open with a specific application
		// This handles .app bundles correctly
		return new ProcessStartInfo("open", $"-a \"{applicationPath}\" \"{filePath}\"");
	}

	protected override string FormatHubArgsCore(string args)
		=> $"-- {args}";

	protected override string? GetScriptingEditorPathCore()
	{
		// Use defaults command to read from Unity's plist
		var process = new ProcessRunner().Run(
			new ProcessStartInfo("defaults", "read com.unity3d.UnityEditor5.x kScriptsDefaultApp")
			{
				RedirectStandardOutput = true,
			});

		var output = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();

		return process.ExitCode == 0 ? output : null;
	}
}
