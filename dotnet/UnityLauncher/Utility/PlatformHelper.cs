using System.Runtime.InteropServices;
using InstallInfo =
	(System.Collections.Generic.IEnumerable<string> editorDefaultLocations, string pathToEditor, string
	hubDefaultLocation);

static class PlatformHelper
{
	// https://docs.unity3d.com/hub/manual/HubCLI.html
	private static readonly Dictionary<OSPlatform, InstallInfo>
		defaultEditorLocations =
			new()
			{
				{
					OSPlatform.OSX, (
						[
							"/Applications/Unity/Hub/Editor/",
						],
						"Unity.app/Contents/MacOS/Unity",
						@"/Applications/Unity\ Hub.app/Contents/MacOS/Unity\ Hub")
				},
				{
					OSPlatform.Windows, (
						[
							@"C:\Program Files\Unity\Hub\Editor\",
						],
						@"Editor\Unity.exe",
						@"C:\Program Files\Unity Hub\Unity Hub.exe")
				},
				{
					OSPlatform.Linux, (
						[
							$"{UserHome}/Unity/Hub/Editor",
							"opt/unity/editors/",
						],
						"Editor/Unity",
						@"~/Applications/Unity\ Hub.AppImage")
				},
			};

	public static string? FindDefaultEditorInstallPath(string version)
	{
		var platformInfo = defaultEditorLocations[GetCurrentOS()];

		// Check for environment variable override first (highest priority)
		var envPath = Environment.GetEnvironmentVariable("UNITY_EDITOR_PATH");
		if (!string.IsNullOrWhiteSpace(envPath))
		{
			string installPath = Path.Combine(envPath, version, platformInfo.pathToEditor);
			if (File.Exists(installPath))
				return installPath;
		}

		// Fall back to default locations
		foreach (string path in platformInfo.editorDefaultLocations)
		{
			string installPath = Path.Combine(path, version, platformInfo.pathToEditor);
			if (File.Exists(installPath))
				return installPath;
		}
		return null;
	}

	public static string? FindDefaultHubInstallPath()
	{
		// Check for environment variable override first (highest priority)
		var envPath = Environment.GetEnvironmentVariable("UNITY_HUB_PATH");
		if (!string.IsNullOrWhiteSpace(envPath))
		{
			var os = GetCurrentOS();
			var hubSubPath = os switch
			{
				_ when os == OSPlatform.OSX => "Contents/MacOS/Unity Hub",
				_ when os == OSPlatform.Windows => "Unity Hub.exe",
				_ when os == OSPlatform.Linux => "Unity Hub.AppImage",
				_ => throw new PlatformNotSupportedException(),
			};

			string hubExecutablePath = Path.Combine(envPath, hubSubPath);
			if (File.Exists(hubExecutablePath))
				return hubExecutablePath;
		}

		// Fall back to default locations
		var platformInfo = defaultEditorLocations[GetCurrentOS()];
		if (File.Exists(platformInfo.hubDefaultLocation))
			return platformInfo.hubDefaultLocation;

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return GetUnityHubPathMacOS();
		}

		return null;
	}

	private static string GetUnityHubPathMacOS()
	{
		var process = new ProcessRunner().Run(
			"mdfind",
			redirectOutput: true,
			"kMDItemCFBundleIdentifier == 'com.unity3d.unityhub'");

		var output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
			throw new Exception("Unity Hub not found on this system");

		var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		if (lines.Length == 0)
			throw new Exception("Unity Hub not found on this system");

		return Path.Combine(lines[0], "Contents", "MacOS", "Unity Hub");
	}

	private static OSPlatform GetCurrentOS()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			return OSPlatform.OSX;
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			return OSPlatform.Windows;
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			return OSPlatform.Linux;
		throw new PlatformNotSupportedException();
	}

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