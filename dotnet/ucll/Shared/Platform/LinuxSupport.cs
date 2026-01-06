sealed class LinuxSupport : PlatformSupport
{
	protected override string GetRelativeEditorPathToExecutableCore()
		=> "Editor/Unity";

	protected override int GetInstallationLevelsToGoUp()
		=> 2;

	protected override string GetEditorPathPattern()
		=> Path.Combine(UserHome, "Unity/Hub/Editor/{0}/Editor/Unity");

	protected override string GetHubPathPattern()
		=> Path.Combine(UserHome, "Applications/Unity Hub.AppImage");

	protected override string GetConfigDirectoryPath()
		=> Path.Combine(UserHome, ".config/UnityHub");

	protected override IEnumerable<string?> GetPlatformSpecificHubPaths()
	{
		// Linux doesn't have platform-specific search like mdfind
		yield break;
	}

	protected override ProcessStartInfo GetUnityProjectSearchProcessCore()
	{
		// Presumably requires manual database update (at least on macOS it's not up-to-date by default).
		return new ProcessStartInfo("bash",
			"-c \"locate ProjectVersion.txt | grep ProjectSettings/ProjectVersion.txt\"");
	}

	protected override ProcessStartInfo GetOpenFileProcessCore(string filePath)
		=> new ProcessStartInfo("xdg-open", filePath);

	protected override ProcessStartInfo GetOpenFileWithApplicationProcessCore(string applicationPath, string filePath)
	{
		// On Linux, directly execute the application with the file as argument
		return new ProcessStartInfo(applicationPath, filePath);
	}

	protected override string FormatHubArgsCore(string args)
		=> args; // Linux doesn't need the "--" prefix

	protected override string? GetScriptingEditorPathCore()
	{
		// Read from Unity's prefs file
		string prefsPath = Path.Combine(UserHome, ".config/unity3d/prefs");

		if (!File.Exists(prefsPath))
			return null;

		foreach (string line in File.ReadLines(prefsPath))
		{
			if (line.StartsWith("kScriptsDefaultApp"))
			{
				// Format: kScriptsDefaultApp: /path/to/editor
				var parts = line.Split(':', 2);
				if (parts.Length == 2)
				{
					return parts[1].Trim();
				}
			}
		}

		return null;
	}
}