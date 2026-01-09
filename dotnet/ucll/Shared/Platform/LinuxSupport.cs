internal sealed class LinuxSupport : PlatformSupport
{
	public override string FormatHubArgs(string args)
		=> args; // Linux doesn't need the "--" prefix

	public override ProcessStartInfo OpenFile(string filePath) => new("xdg-open", filePath);

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

		// Format: kScriptsDefaultApp: /path/to/editor
		return File.ReadLines(prefsPath)
			.Where(line => line.StartsWith("kScriptsDefaultApp"))
			.Select(line => line.Split(':', 2))
			.Where(parts => parts.Length == 2)
			.Select(parts => parts[1].Trim()).FirstOrDefault();
	}
}