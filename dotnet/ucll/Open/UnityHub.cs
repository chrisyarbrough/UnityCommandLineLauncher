using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

record EditorInfo(string Version, string Path);

partial class UnityHub(IProcessRunner mutatingProcessRunner)
{
	private static string? _hubPathCache;
	private static List<EditorInfo>? _editorsCache;

	/// <summary>
	/// Returns the path to the editor executable on the current platform.
	/// </summary>
	public static string GetEditorPath(string version)
	{
		// Fast path: check default install location first
		var fastPath = PlatformHelper.FindDefaultEditorInstallPath(version);
		if (fastPath != null)
			return fastPath;

		// Fallback: query Unity Hub for custom installation locations
		var editors = ListInstalledEditors();
		string? path = editors.FirstOrDefault(e => e.Version == version)?.Path;

		if (path == null)
			throw new Exception($"Unity version {version} is not installed.");

		return path;
	}

	public static IEnumerable<string> GetRecentProjects(bool favoritesOnly = false)
	{
		var configDir = PlatformHelper.GetUnityHubConfigDirectory();
		var projectsFile = Path.Combine(configDir, "projects-v1.json");

		try
		{
			var json = File.ReadAllText(projectsFile);
			var root = JsonNode.Parse(json);
			var data = root?["data"]?.AsObject()!;

			var projects = new List<(string path, long lastModified, bool isFavorite)>();

			foreach ((string projectPath, JsonNode? value) in data)
			{
				var project = value?.AsObject()!;
				var lastModified = project["lastModified"]?.GetValue<long>();
				if (lastModified.HasValue)
				{
					var isFavorite = project["isFavorite"]?.GetValue<bool>() ?? false;
					projects.Add((projectPath, lastModified.Value, isFavorite));
				}
			}

			var filteredProjects = favoritesOnly
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

	public void EnsureEditorInstalled(string version, string? changeset, string[]? additionalArgs = null)
	{
		if (IsEditorInstalled(version))
			return;

		if (changeset == null)
		{
			WriteStatusUpdate("Changeset not provided, fetching from Unity API");
			changeset = UnityReleaseApi.FetchChangesetAsync(version).Result;
		}

		InstallEditor(version, changeset, additionalArgs ?? []);
	}

	private static List<EditorInfo> ListInstalledEditors()
	{
		if (_editorsCache != null)
			return _editorsCache;

		var hubPath = GetUnityHubPath();

		var process = ProcessRunner.Default.Run(
			new ProcessStartInfo(hubPath, PlatformHelper.FormatHubArgs("--headless editors --installed"))
				{ RedirectStandardOutput = true, RedirectStandardError = true });
		var output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		if (process.ExitCode != 0)
		{
			throw new Exception(
				$"Failed to list installed editors. Exit code: {process.ExitCode} Error output: {process.StandardError.ReadToEnd()}");
		}

		_editorsCache = ParseEditorsOutput(output);
		return _editorsCache;
	}

	[GeneratedRegex(@"([0-9]+\.[0-9]+\.[0-9]+[a-z0-9]*)\s+\(.*\)\s+installed\s+at\s+(.+)")]
	private static partial Regex EditorLineRegex();

	private static List<EditorInfo> ParseEditorsOutput(string output)
	{
		var editors = new List<EditorInfo>();
		var regex = EditorLineRegex();

		foreach (var line in output.Split('\n'))
		{
			var match = regex.Match(line);
			if (match.Success)
			{
				editors.Add(new EditorInfo(
					match.Groups[1].Value,
					match.Groups[2].Value
				));
			}
		}

		return editors;
	}

	private static string GetUnityHubPath()
	{
		if (_hubPathCache != null)
			return _hubPathCache;

		_hubPathCache = PlatformHelper.FindDefaultHubInstallPath();

		if (_hubPathCache == null)
			throw new Exception("Unity Hub not found.");

		return _hubPathCache;
	}

	private static bool IsEditorInstalled(string version)
	{
		// Fast path: check default install location first
		if (PlatformHelper.FindDefaultEditorInstallPath(version) != null)
			return true;

		// Fallback: query Unity Hub for custom installation locations
		try
		{
			var editors = ListInstalledEditors();
			return editors.Any(e => e.Version == version);
		}
		catch
		{
			return false;
		}
	}

	private void InstallEditor(string version, string changeset, string[] additionalArgs)
	{
		var hubPath = GetUnityHubPath();

		// ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
		var arch = RuntimeInformation.ProcessArchitecture switch
		{
			Architecture.X64 => "x86_64",
			Architecture.Arm64 => "arm64",
			_ => throw new PlatformNotSupportedException(
				$"Unsupported architecture: {RuntimeInformation.ProcessArchitecture}"),
		};

		WriteStatusUpdate($"Installing Unity version {version} {changeset}");

		var args = $"--headless install --version {version} --changeset {changeset} --architecture {arch}";
		if (additionalArgs.Length > 0)
			args += " " + string.Join(" ", additionalArgs);

		var process = mutatingProcessRunner.Run(new ProcessStartInfo(
			hubPath,
			PlatformHelper.FormatHubArgs(args)) { RedirectStandardError = true });
		process.WaitForExit();

		if (process.ExitCode != 0)
		{
			throw new Exception($"Failed to install Unity {version}. (Exit code: {process.ExitCode})");
		}

		// Invalidate the cache after installing a new editor
		_editorsCache = null;
	}

	private static void WriteStatusUpdate(string message)
	{
		// This output is colored to differentiate it from the verbose Unity Hub output, which can be noisy.
		// However, we do want the progress feedback that the Hub provides.
		AnsiConsole.MarkupLine($"[cyan]{message}...[/]");
	}
}