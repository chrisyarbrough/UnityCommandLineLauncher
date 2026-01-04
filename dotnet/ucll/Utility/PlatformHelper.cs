using System.Runtime.InteropServices;

static class PlatformHelper
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
}