using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

record EditorInfo(string Version, string Path);

partial class UnityHub(IProcessRunner modifyingProcessRunner)
{
	private static string? _hubPathCache;
	private static List<EditorInfo>? _editorsCache;
	private static IProcessRunner readOnlyProcessRunner = new ProcessRunner();

	public static string GetEditorPath(string version)
	{
		var editors = ListInstalledEditors();
		string? path = editors.FirstOrDefault(e => e.Version == version)?.Path;

		if (path == null)
			throw new Exception($"Unity version {version} is not installed.");

		return path;
	}

	public async Task EnsureEditorInstalledAsync(string version, string? changeset)
	{
		if (IsEditorInstalled(version))
			return;

		if (changeset == null)
		{
			AnsiConsole.MarkupLine("Changeset not provided, fetching from Unity API...");
			changeset = await UnityReleaseApi.FetchChangesetAsync(version);
		}

		InstallEditor(version, changeset);
	}

	private static List<EditorInfo> ListInstalledEditors()
	{
		if (_editorsCache != null)
			return _editorsCache;

		var hubPath = GetUnityHubPath();

		var process = readOnlyProcessRunner.Run(hubPath, redirectOutput: true, "-- --headless editors --installed");
		var output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		if (process.ExitCode != 0)
			throw new Exception($"Failed to list installed editors: {process.StandardError.ReadToEnd()}");

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

		var process = readOnlyProcessRunner.Run(
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

		var hubPath = Path.Combine(lines[0], "Contents", "MacOS", "Unity Hub");
		_hubPathCache = hubPath;
		return hubPath;
	}

	private static bool IsEditorInstalled(string version)
	{
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

	private void InstallEditor(string version, string changeset)
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

		AnsiConsole.MarkupLine($"Installing Unity version {version} {changeset}...");

		var process = modifyingProcessRunner.Run(
			hubPath,
			redirectOutput: true,
			$"-- --headless install --version {version} --changeset {changeset} --architecture {arch}");
		process.WaitForExit();

		if (process.ExitCode != 0)
		{
			var error = process.StandardError.ReadToEnd();
			throw new Exception($"Failed to install Unity {version}: {error}");
		}

		// Invalidate the cache after installing a new editor
		_editorsCache = null;
	}
}