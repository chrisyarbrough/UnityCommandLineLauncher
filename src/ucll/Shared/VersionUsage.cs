using System.Text.Json;

internal class VersionUsage
{
	public HashSet<string> Installed { get; }
	public HashSet<string> Used { get; }
	public HashSet<string> UsedNotInstalled => Used.Except(Installed).ToHashSet();
	public HashSet<string> InstalledNotUsed => Installed.Except(Used).ToHashSet();

	private readonly PlatformSupport platformSupport;
	private readonly UnityHub unityHub;

	public VersionUsage(PlatformSupport platformSupport, UnityHub unityHub)
	{
		this.platformSupport = platformSupport;
		this.unityHub = unityHub;
		Installed = unityHub.ListInstalledEditors()
			.Select(info => info.Version)
			.ToHashSet();

		Used = FindUnityProjects(platformSupport)
			.Select(p => Project.Parse(p).Version)
			.ToHashSet();
	}

	public static IEnumerable<string> FindUnityProjects(PlatformSupport platformSupport)
	{
		var startInfo = platformSupport.GetUnityProjectSearchProcess();
		startInfo.RedirectStandardOutput = true;
		var process = ProcessRunner.Default.Run(startInfo);
		process.WaitForExit();

		while (!process.StandardOutput.EndOfStream)
			yield return process.StandardOutput.ReadLine()!;
	}

	public string[] GetInstalledModules(string version)
	{
		string editorPath = unityHub.GetEditorPath(version);
		string installationRoot = platformSupport.FindInstallationRoot(editorPath);
		string modulesJsonPath = Path.Combine(installationRoot, "modules.json");

		if (!File.Exists(modulesJsonPath))
			return [];

		string json = File.ReadAllText(modulesJsonPath);

		using JsonDocument doc = JsonDocument.Parse(json);

		return doc.RootElement
			.EnumerateArray()
			.Where(module => module.TryGetProperty("selected", out var selected) && selected.GetBoolean())
			.Where(module => module.TryGetProperty("visible", out var visible) && visible.GetBoolean())
			.Where(module => module.TryGetProperty("name", out _))
			.Select(module => module.GetProperty("name").GetString())
			.Where(name => name != null)
			.Cast<string>()
			.ToArray();
	}
}