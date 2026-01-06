abstract class PlatformSupport
{
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

	#region Public Instance API

	public string GetRelativeEditorPathToExecutable()
		=> GetRelativeEditorPathToExecutableCore();

	public string GetInstallationRootDirectory(string editorPath)
	{
		int levelsToGoUp = GetInstallationLevelsToGoUp();

		string? dir = editorPath;
		for (int i = 0; i < levelsToGoUp; i++)
		{
			dir = Path.GetDirectoryName(dir);
			if (dir == null)
				throw new Exception($"Unable to determine installation directory from path: {editorPath}");
		}

		return dir;
	}

	public string? FindDefaultEditorInstallPath(string version)
		=> FindFirstValidPath(GetEditorPathCandidatePatterns(), pattern => string.Format(pattern, version));

	private IEnumerable<string?> GetEditorPathCandidatePatterns()
	{
		yield return Environment.GetEnvironmentVariable("UNITY_EDITOR_PATH");
		yield return GetEditorPathPattern();
	}

	public string GetUnityHubConfigDirectory()
		=> GetConfigDirectoryPath();

	public string? FindDefaultHubInstallPath()
		=> FindFirstValidPath(GetHubPathCandidates());

	private IEnumerable<string?> GetHubPathCandidates()
	{
		yield return Environment.GetEnvironmentVariable("UNITY_HUB_PATH");
		yield return GetHubPathPattern();

		foreach (var path in GetPlatformSpecificHubPaths())
			yield return path;
	}

	public string FormatHubArgs(string args)
		=> FormatHubArgsCore(args);

	public ProcessStartInfo GetUnityProjectSearchProcess()
		=> GetUnityProjectSearchProcessCore();

	public ProcessStartInfo GetOpenFileProcess(string filePath)
		=> GetOpenFileProcessCore(filePath);

	public ProcessStartInfo GetOpenFileWithApplicationProcess(string applicationPath, string filePath)
		=> GetOpenFileWithApplicationProcessCore(applicationPath, filePath);

	public string? GetUnityScriptingEditorPath()
	{
		try
		{
			string? editorPath = GetScriptingEditorPathCore();

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

	private string? FindFirstValidPath(IEnumerable<string?> paths, Func<string, string>? processor = null)
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