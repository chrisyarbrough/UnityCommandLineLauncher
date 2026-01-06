using System.Runtime.InteropServices;

static class PlatformSupport
{
	// https://docs.unity3d.com/6000.3/Documentation/Manual/EditorCommandLineArguments.html
	// https://docs.unity3d.com/hub/manual/HubCLI.html
	private static readonly Dictionary<OSPlatform, (string editor, string hub, string config)> installInfo = new()
	{
		{
			OSPlatform.OSX, (
				editor: "/Applications/Unity/Hub/Editor/{0}/Unity.app/Contents/MacOS/Unity",
				hub: "/Applications/Unity Hub.app/Contents/MacOS/Unity Hub",
				config: Path.Combine(UserHome, "Library/Application Support/UnityHub"))
		},
		{
			OSPlatform.Windows, (
				editor: @"C:\Program Files\Unity\Hub\Editor\{0}\Editor\Unity.exe",
				hub: @"C:\Program Files\Unity Hub\Unity Hub.exe",
				config: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UnityHub"))
		},
		{
			OSPlatform.Linux, (
				editor: Path.Combine(UserHome, "/Unity/Hub/Editor/{0}/Editor/Unity"),
				hub: Path.Combine(UserHome, "/Applications/Unity Hub.AppImage"),
				config: Path.Combine(UserHome, ".config/UnityHub"))
		},
	};

	// The path from what UnityHub considers the "install directory" to the platform executable.
	public static string GetRelativeEditorPathToExecutable()
	{
		OSPlatform platform = GetCurrentOS();
		if (platform == OSPlatform.OSX)
			return "Contents/MacOS/Unity";
		else if (platform == OSPlatform.Windows)
			return @"Editor\Unity.exe";
		else
			return "Editor/Unity";
	}

	// Accepts a path that was provided by UnityHub.GetEditorPath().
	public static string GetInstallationRootDirectory(string editorPath)
	{
		// Extract the version directory from the editor path
		// macOS: /Applications/Unity/Hub/Editor/{version}/Unity.app/Contents/MacOS/Unity -> go up 4 levels
		// Windows: C:\Program Files\Unity\Hub\Editor\{version}\Editor\Unity.exe -> go up 2 levels
		// Linux: ~/Unity/Hub/Editor/{version}/Editor/Unity -> go up 2 levels
		// Remember, the version is only part of default installations, but it could very well be a custom name entirely.

		int levelsToGoUp = GetCurrentOS() == OSPlatform.OSX ? 4 : 2;

		string? dir = editorPath;
		for (int i = 0; i < levelsToGoUp; i++)
		{
			dir = Path.GetDirectoryName(dir);
			if (dir == null)
				throw new Exception($"Unable to determine installation directory from path: {editorPath}");
		}

		return dir;
	}

	public static string? FindDefaultEditorInstallPath(string version)
		=> FindFirstValidPath(GetEditorPathCandidatePatterns(), pattern => string.Format(pattern, version));

	private static IEnumerable<string?> GetEditorPathCandidatePatterns()
	{
		yield return Environment.GetEnvironmentVariable("UNITY_EDITOR_PATH");
		yield return installInfo[GetCurrentOS()].editor;
	}

	public static string? FindDefaultHubInstallPath()
		=> FindFirstValidPath(GetHubPathCandidates());

	public static string GetUnityHubConfigDirectory()
		=> installInfo[GetCurrentOS()].config;

	private static IEnumerable<string?> GetHubPathCandidates()
	{
		yield return Environment.GetEnvironmentVariable("UNITY_HUB_PATH");
		yield return installInfo[GetCurrentOS()].hub;

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			yield return FindUnityHubPathMacOS();
	}

	private static string? FindFirstValidPath(IEnumerable<string?> paths, Func<string, string>? processor = null)
	{
		foreach (string? path in paths)
		{
			if (string.IsNullOrWhiteSpace(path))
				continue;

			string processedPath = processor?.Invoke(path) ?? path;
			if (File.Exists(processedPath))
				return processedPath;
		}

		return null;
	}

	private static string FindUnityHubPathMacOS()
	{
		var process = new ProcessRunner().Run(
			new ProcessStartInfo("mdfind", "kMDItemCFBundleIdentifier == 'com.unity3d.unityhub'")
			{
				RedirectStandardOutput = true,
			});

		var output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
			throw new Exception("Unity Hub not found on this system");

		var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		if (lines.Length == 0)
			throw new Exception("Unity Hub not found on this system");

		return Path.Combine(lines[0], "Contents", "MacOS", "Unity Hub");
	}

	private static readonly OSPlatform[] supportedPlatforms =
	[
		OSPlatform.OSX,
		OSPlatform.Windows,
		OSPlatform.Linux,
	];

	private static OSPlatform GetCurrentOS() => supportedPlatforms.First(RuntimeInformation.IsOSPlatform);

	private static string UserHome => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

	/// <summary>
	/// Formats Unity Hub arguments with the appropriate command separator for the current platform.
	/// macOS and Windows require "--" before the arguments, while Linux does not.
	/// </summary>
	public static string FormatHubArgs(string args)
	{
		var os = GetCurrentOS();
		var needsDashes = os == OSPlatform.OSX || os == OSPlatform.Windows;
		return needsDashes ? $"-- {args}" : args;
	}

	/// <summary>
	/// Returns a ProcessStartInfo configured to search the OS index for Unity projects on the current platform.
	/// </summary>
	public static ProcessStartInfo GetUnityProjectSearchProcess()
	{
		var os = GetCurrentOS();

		if (os == OSPlatform.OSX)
		{
			// Automatically indexed Spotlight search.
			return new ProcessStartInfo("bash",
				"-c \"mdfind 'kMDItemFSName == ProjectVersion.txt' | grep ProjectSettings/ProjectVersion.txt\"");
		}

		if (os == OSPlatform.Windows)
		{
			// Use Windows Search index via COM (equivalent to mdfind on macOS)
			// Queries the SystemIndex database - very fast, uses existing Windows Search index
			// Note: Only searches indexed locations (configurable in Windows Search settings)
			const string script = """
			                      $connection = New-Object -ComObject ADODB.Connection
			                      $recordset = New-Object -ComObject ADODB.Recordset
			                      try {
			                      	$connection.Open('Provider=Search.CollatorDSO;Extended Properties=''Application=Windows'';')
			                      	$query = 'SELECT System.ItemPathDisplay FROM SystemIndex WHERE System.FileName = ''ProjectVersion.txt'''
			                      	$recordset.Open($query, $connection)
			                      	while (-not $recordset.EOF) {
			                      		$path = $recordset.Fields.Item('System.ItemPathDisplay').Value
			                      		if ($path -like '*\ProjectSettings\ProjectVersion.txt') {
			                      			$path
			                      		}
			                      		$recordset.MoveNext()
			                      	}
			                      } finally {
			                      	if ($recordset.State -eq 1) { $recordset.Close() }
			                      	if ($connection.State -eq 1) { $connection.Close() }
			                      }
			                      """;

			return new ProcessStartInfo("powershell.exe", $"-NoProfile -Command \"{script}\"");
		}

		else if (os == OSPlatform.Linux)
		{
			// Presumably requires manual database update (at least on macOS it's not up-to-date by default).
			return new ProcessStartInfo("bash",
				"-c \"locate ProjectVersion.txt | grep ProjectSettings/ProjectVersion.txt\"");
		}

		throw new NotSupportedException($"Unsupported OS: {os}");
	}

	/// <summary>
	/// Returns a ProcessStartInfo configured to open a file with the OS default application.
	/// </summary>
	public static ProcessStartInfo GetOpenFileProcess(string filePath)
	{
		var os = GetCurrentOS();

		if (os == OSPlatform.OSX)
		{
			return new ProcessStartInfo("open", filePath);
		}

		if (os == OSPlatform.Windows)
		{
			// Use cmd /c start with empty window title
			return new ProcessStartInfo("cmd.exe", $"/c start \"\" \"{filePath}\"");
		}

		if (os == OSPlatform.Linux)
		{
			return new ProcessStartInfo("xdg-open", filePath);
		}

		throw new NotSupportedException($"Unsupported OS: {os}");
	}

	/// <summary>
	/// Returns a ProcessStartInfo configured to open a file with a specific application.
	/// </summary>
	public static ProcessStartInfo GetOpenFileWithApplicationProcess(string applicationPath, string filePath)
	{
		var os = GetCurrentOS();

		if (os == OSPlatform.OSX)
		{
			// On macOS, use 'open -a' to open with a specific application
			// This handles .app bundles correctly
			return new ProcessStartInfo("open", $"-a \"{applicationPath}\" \"{filePath}\"");
		}

		if (os == OSPlatform.Windows)
		{
			// On Windows, directly execute the application with the file as argument
			return new ProcessStartInfo(applicationPath, $"\"{filePath}\"");
		}

		if (os == OSPlatform.Linux)
		{
			// On Linux, directly execute the application with the file as argument
			return new ProcessStartInfo(applicationPath, filePath);
		}

		throw new NotSupportedException($"Unsupported OS: {os}");
	}

	/// <summary>
	/// Gets the scripting editor path from Unity preferences.
	/// Returns null if the preference is not set.
	/// </summary>
	public static string? GetUnityScriptingEditorPath()
	{
		var os = GetCurrentOS();

		try
		{
			string? editorPath = null;

			if (os == OSPlatform.OSX)
				editorPath = GetScriptingEditorMacOS();
			else if (os == OSPlatform.Windows)
				editorPath = GetScriptingEditorWindows();
			else if (os == OSPlatform.Linux)
				editorPath = GetScriptingEditorLinux();

			if (!string.IsNullOrWhiteSpace(editorPath))
				return editorPath;
		}
		catch
		{
			// If we can't read preferences, return null and fall back to default behavior
		}

		return null;
	}

	private static string? GetScriptingEditorMacOS()
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

	private static string? GetScriptingEditorWindows()
	{
		// Read from Windows registry
		const string script = """
		                      try {
		                      	$path = Get-ItemProperty -Path 'HKCU:\Software\Unity Technologies\Unity Editor 5.x' -Name 'kScriptsDefaultApp' -ErrorAction Stop
		                      	$path.'kScriptsDefaultApp'
		                      } catch {
		                      	exit 1
		                      }
		                      """;

		var process = new ProcessRunner().Run(
			new ProcessStartInfo("powershell.exe", $"-NoProfile -Command \"{script}\"")
			{
				RedirectStandardOutput = true,
			});

		var output = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();

		return process.ExitCode == 0 ? output : null;
	}

	private static string? GetScriptingEditorLinux()
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