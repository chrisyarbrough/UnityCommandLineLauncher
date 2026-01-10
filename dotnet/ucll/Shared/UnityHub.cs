using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using EditorInfo = (string Version, string Path);

internal class UnityHub(PlatformSupport platformSupport)
{
	private readonly Lazy<string> hubPathCache = new(() => platformSupport.FindHubInstallPath() ?? throw new UserException(
		"Unity Hub not found. If it is installed in a custom location, configure the UNITY_HUB_PATH environment variable."));

	// It would seem more efficient to store the editors in a Dictionary by version, but it's possible
	// to install multiple editors with the same version (e.g. Intel and Silicon on macOS).
	private List<EditorInfo>? editorsCache;

	/// <summary>
	/// Returns the path to the Unity Hub executable.
	/// </summary>
	public string GetHubPath() => hubPathCache.Value;

	/// <summary>
	/// Returns the path to the editor executable on the current platform.
	/// </summary>
	public string GetEditorPath(string version)
	{
		// Fast: try the default install location first.
		string? executablePath = platformSupport.FindDefaultEditorPath(version);
		if (executablePath != null)
			return executablePath;

		// Fallback: query Unity Hub for custom installation locations.
		var editors = ListInstalledEditors();
		string? appBundlePath = editors.FirstOrDefault(p => p.Version == version).Path;

		if (appBundlePath == null)
			throw new UserException($"Unity version {version} is not installed.");

		return Path.Combine(appBundlePath, platformSupport.RelativeEditorPathToExecutable);
	}

	public IEnumerable<string> GetRecentProjects(bool favoriteOnly = false)
	{
		string configDir = platformSupport.UnityHubConfigDirectory;
		string projectsFile = Path.Combine(configDir, "projects-v1.json");

		try
		{
			string json = File.ReadAllText(projectsFile);
			var root = JsonNode.Parse(json);
			var data = root?["data"]?.AsObject()!;

			var projects = new List<(string path, long lastModified, bool isFavorite)>();

			foreach ((string projectPath, JsonNode? value) in data)
			{
				var project = value?.AsObject()!;
				long? lastModified = project["lastModified"]?.GetValue<long>();
				if (lastModified.HasValue)
				{
					bool isFavorite = project["isFavorite"]?.GetValue<bool>() ?? false;
					projects.Add((projectPath, lastModified.Value, isFavorite));
				}
			}

			var filteredProjects = favoriteOnly
				? projects.Where(p => p.isFavorite)
				: projects;

			return filteredProjects
				.OrderByDescending(p => p.lastModified)
				.Select(p => p.path);
		}
		catch
		{
			return [];
		}
	}

	public void InstallEditorChecked(
		string version,
		string? changeset,
		IProcessRunner processRunner,
		string[]? additionalArgs = null)
	{
		if (IsEditorInstalled(version))
			return;

		InstallEditor(version, changeset, processRunner, additionalArgs ?? []);
	}

	public void InstallEditor(string version, string? changeset, IProcessRunner processRunner, string[] additionalArgs)
	{
		if (changeset == null)
		{
			WriteStatusUpdate("Changeset not provided, fetching from Unity API");
			changeset = UnityReleaseApi.FetchChangesetAsync(version).Result;
		}

		WriteStatusUpdate($"Installing Unity version {version} {changeset}");

		string args = $"--headless install --version {version} --changeset {changeset}";
		args = ConfigurePlatformArgs(args);

		if (additionalArgs.Length > 0)
			args += " " + string.Join(" ", additionalArgs);

		var process = processRunner.Run(new ProcessStartInfo(
			hubPathCache.Value,
			platformSupport.FormatHubArgs(args)) { RedirectStandardError = true });
		process.WaitForExit();

		if (process.ExitCode != 0)
		{
			throw new UserException($"Failed to install Unity {version}. (Exit code: {process.ExitCode})");
		}

		// Invalidate the cache after installing a new editor
		editorsCache = null;
	}

	public List<EditorInfo> ListInstalledEditors()
	{
		if (editorsCache != null)
			return editorsCache;

		string args = platformSupport.FormatHubArgs("--headless editors --installed");
		ProcessStartInfo startInfo = new(hubPathCache.Value, args)
		{
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		};
		Process process = ProcessRunner.Default.Run(startInfo);
		string output = process.CaptureOutput().output;

		// There's a bug in some older Unity Hub version where the exit code is non-zero, but the output works.
		// So, don't do any validation, just attempt to parse.

		// The paths in this output specify the Unity.app directory on macOS, not the executable within.
		editorsCache = ParseEditorsOutput(output);
		return editorsCache;
	}

	internal static List<EditorInfo> ParseEditorsOutput(string output)
	{
		var matches = Regex.Matches(output, @"(\S+).*? installed at (.+)");
		return matches.Select(m => new EditorInfo(m.Groups[1].Value, m.Groups[2].Value)).ToList();
	}

	private bool IsEditorInstalled(string version)
	{
		// Fast path: check default install location first
		if (platformSupport.FindDefaultEditorPath(version) != null)
			return true;

		// Fallback: query Unity Hub for custom installation locations
		try
		{
			var editors = ListInstalledEditors();
			return editors.Any(s => s.Version == version);
		}
		catch
		{
			return false;
		}
	}

	private static string ConfigurePlatformArgs(string args)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			string? arch = RuntimeInformation.ProcessArchitecture switch
			{
				Architecture.X64 => "x86_64",
				Architecture.Arm64 => "arm64",
				// Unsupported architecture will probably cause the hub installation to fail,
				// but better we try and let the user discover that and report a bug than abort.
				_ => null,
			};
			if (arch != null)
				return args + $" --architecture {arch}";
		}
		return args;
	}

	private static void WriteStatusUpdate(string message)
	{
		// This output is colored to differentiate it from the verbose Unity Hub output, which can be noisy.
		// However, we do want the progress feedback that the Hub provides.
		AnsiConsole.MarkupLine($"[cyan]{message}...[/]");
	}
}