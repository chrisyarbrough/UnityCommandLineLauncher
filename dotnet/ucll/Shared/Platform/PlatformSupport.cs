using System.Runtime.InteropServices;

abstract class PlatformSupport
{
	private static readonly Lazy<PlatformSupport> _instance = new(() =>
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			return new MacSupport();

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			return new WindowsSupport();

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			return new LinuxSupport();

		throw new PlatformNotSupportedException($"Unsupported platform.");
	});

	private static PlatformSupport Instance => _instance.Value;

	#region Abstract Template Methods

	protected abstract string GetRelativeEditorPathToExecutableCore();
	protected abstract int GetInstallationLevelsToGoUp();
	protected abstract string GetEditorPathPattern();
	protected abstract string GetHubPathPattern();
	protected abstract string GetConfigDirectoryPath();
	protected abstract IEnumerable<string?> GetPlatformSpecificHubPaths();
	protected abstract ProcessStartInfo GetUnityProjectSearchProcessCore();
	protected abstract ProcessStartInfo GetOpenFileProcessCore(string filePath);
	protected abstract ProcessStartInfo GetOpenFileWithApplicationProcessCore(string applicationPath, string filePath);
	protected abstract string FormatHubArgsCore(string args);
	protected abstract string? GetScriptingEditorPathCore();

	#endregion

	#region Public Static API (Facades)

	public static string GetRelativeEditorPathToExecutable()
		=> Instance.GetRelativeEditorPathToExecutableCore();

	public static string GetInstallationRootDirectory(string editorPath)
	{
		int levelsToGoUp = Instance.GetInstallationLevelsToGoUp();

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
		yield return Instance.GetEditorPathPattern();
	}

	public static string GetUnityHubConfigDirectory()
		=> Instance.GetConfigDirectoryPath();

	public static string? FindDefaultHubInstallPath()
		=> FindFirstValidPath(GetHubPathCandidates());

	private static IEnumerable<string?> GetHubPathCandidates()
	{
		yield return Environment.GetEnvironmentVariable("UNITY_HUB_PATH");
		yield return Instance.GetHubPathPattern();

		foreach (var path in Instance.GetPlatformSpecificHubPaths())
			yield return path;
	}

	public static string FormatHubArgs(string args)
		=> Instance.FormatHubArgsCore(args);

	public static ProcessStartInfo GetUnityProjectSearchProcess()
		=> Instance.GetUnityProjectSearchProcessCore();

	public static ProcessStartInfo GetOpenFileProcess(string filePath)
		=> Instance.GetOpenFileProcessCore(filePath);

	public static ProcessStartInfo GetOpenFileWithApplicationProcess(string applicationPath, string filePath)
		=> Instance.GetOpenFileWithApplicationProcessCore(applicationPath, filePath);

	public static string? GetUnityScriptingEditorPath()
	{
		try
		{
			string? editorPath = Instance.GetScriptingEditorPathCore();

			if (!string.IsNullOrWhiteSpace(editorPath))
				return editorPath;
		}
		catch
		{
			// If we can't read preferences, return null and fall back to default behavior
		}

		return null;
	}

	#endregion

	#region Shared Utility Methods

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

	protected static string UserHome => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

	#endregion
}