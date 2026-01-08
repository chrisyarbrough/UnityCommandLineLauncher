sealed class LinuxSupport : PlatformSupport
{
	public override string FormatHubArgs(string args)
		=> args; // Linux doesn't need the "--" prefix

	public override ProcessStartInfo OpenFile(string filePath)
		=> new ProcessStartInfo("xdg-open", filePath);

	public override ProcessStartInfo OpenFileWithApp(string filePath, string applicationPath)
	{
		// On Linux, directly execute the application with the file as argument
		return new ProcessStartInfo(applicationPath, filePath);
	}

	public override string RelativeEditorPathToExecutable => "Editor/Unity";

	public override string UnityHubConfigDirectory => Path.Combine(UserHome, ".config/UnityHub");

	public override ProcessStartInfo GetUnityProjectSearchProcess()
	{
		// Presumably requires manual database update (at least on macOS it's not up-to-date by default).
		return new ProcessStartInfo("bash",
			"-c \"locate ProjectVersion.txt | grep ProjectSettings/ProjectVersion.txt\"");
	}

	protected override string DefaultEditorPathTemplate => Path.Combine(UserHome, "Unity/Hub/Editor/{0}/Editor/Unity");

	protected override string DefaultUnityHubPath => Path.Combine(UserHome, "Applications/Unity Hub.AppImage");

	public override string? GetUnityScriptingEditorPath()
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