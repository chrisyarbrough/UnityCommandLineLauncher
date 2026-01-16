using System.Runtime.InteropServices;

internal abstract class PlatformSupport
{
	public static PlatformSupport Create()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			return new MacSupport();

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			return new WindowsSupport();

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			return new LinuxSupport();

		throw new UserException($"Unsupported platform: {RuntimeInformation.RuntimeIdentifier}");
	}

	/// <summary>
	/// Default path to the editor executable or null if it doesn't exist.
	/// </summary>
	public string? FindDefaultEditorPath(string version)
		=> AllDefaultEditorPathTemplates()
			.Where(p => p != null)
			.Select(p => string.Format(p!, version))
			.FirstOrDefault(File.Exists);

	private IEnumerable<string?> AllDefaultEditorPathTemplates()
	{
		yield return Environment.GetEnvironmentVariable("UNITY_EDITOR_PATH");
		yield return DefaultEditorPathTemplate;
	}

	/// <summary>
	/// Given the path to an editor executable, returns the root directory path of the installation.
	/// </summary>
	public string FindInstallationRoot(string editorPath)
		=> new DirectoryInfo(editorPath.Replace(RelativeEditorPathToExecutable, string.Empty)).Parent!.FullName;

	/// <summary>
	/// Path to the Unity Hub executable or null if it doesn't exist (or couldn't be found).
	/// </summary>
	public string? FindHubInstallPath()
		=> AllDefaultHubInstallPaths()
			.FirstOrDefault(File.Exists);

	private IEnumerable<string?> AllDefaultHubInstallPaths()
	{
		yield return Environment.GetEnvironmentVariable("UNITY_HUB_PATH");
		yield return DefaultUnityHubPath;

		if (FindUnityHub() is { } unityHub)
			yield return unityHub;
	}

	/// <summary>
	/// Prefixes the Unity Hub arguments with a double-dash by default.
	/// </summary>
	public virtual string FormatHubArgs(string args) => $"-- {args}";

	/// <summary>
	/// A system process that opens the file with the default app association.
	/// </summary>
	public abstract ProcessStartInfo OpenFile(string filePath);

	/// <summary>
	/// A system process that launches an app (like the scripting editor).
	/// </summary>
	public abstract ProcessStartInfo OpenFileWithApp(string filePath, string applicationPath);

	/// <summary>
	/// The path from the installation bundle (macOS) or root (Windows) to the executable.
	/// </summary>
	public abstract string RelativeEditorPathToExecutable { get; }

	/// <summary>
	/// The path to the directory that contains Unity Hub config files.
	/// </summary>
	public abstract string UnityHubConfigDirectory { get; }

	/// <summary>
	/// A system process that returns all paths to ProjectSettings/ProjectVersion.txt files on the system.
	/// </summary>
	public abstract ProcessStartInfo GetUnityProjectSearchProcess();

	/// <summary>
	/// Returns the path to the script editor set in the Unity preferences.
	/// </summary>
	public abstract string? GetUnityScriptingEditorPath();

	/// <summary>
	/// Path to the editor executable with a placeholder for the version directory.
	/// E.g.: /Applications/Unity/Hub/Editor/{0}/Unity.app/Contents/MacOS/Unity
	/// </summary>
	protected abstract string DefaultEditorPathTemplate { get; }

	/// <summary>
	/// Default path to the Unity Hub executable.
	/// </summary>
	protected abstract string DefaultUnityHubPath { get; }

	/// <summary>
	/// Searches the system for the path to the Unity Hub executable (if feasible on this platform).
	/// </summary>
	protected virtual string? FindUnityHub() => null;

	protected static string UserHome => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
}