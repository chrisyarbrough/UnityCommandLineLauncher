using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Spectre.Console;

record EditorInfo(string Version, string Path);

static partial class UnityHub
{
	private static string? _hubPathCache;

	[GeneratedRegex(@"([0-9]+\.[0-9]+\.[0-9]+[a-z0-9]*)\s+\(.*\)\s+installed\s+at\s+(.+)")]
	private static partial Regex EditorLineRegex();

	public static string GetUnityHubPath()
	{
		if (_hubPathCache != null)
			return _hubPathCache;

		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "mdfind",
				Arguments = "kMDItemCFBundleIdentifier == 'com.unity3d.unityhub'",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
			},
		};

		process.Start();
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

	public static List<EditorInfo> ListInstalledEditors()
	{
		var hubPath = GetUnityHubPath();
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = hubPath,
				Arguments = "-- --headless editors --installed",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
			},
		};

		process.Start();
		var output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		if (process.ExitCode != 0)
			throw new Exception($"Failed to list installed editors: {process.StandardError.ReadToEnd()}");

		return ParseEditorsOutput(output);
	}

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

	public static string? GetEditorPath(string version)
	{
		var editors = ListInstalledEditors();
		return editors.FirstOrDefault(e => e.Version == version)?.Path;
	}

	public static bool IsEditorInstalled(string version)
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

	public static void InstallEditor(string version, string changeset)
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

		AnsiConsole.MarkupLine($"[cyan]Installing Unity version {version} {changeset}...[/]");

		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = hubPath,
				Arguments = $"-- --headless install --version {version} --changeset {changeset} --architecture {arch}",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
			},
		};

		process.Start();
		process.WaitForExit();

		if (process.ExitCode != 0)
		{
			var error = process.StandardError.ReadToEnd();
			throw new Exception($"Failed to install Unity {version}: {error}");
		}
	}

	public static async Task EnsureEditorInstalledAsync(string version, string? changeset)
	{
		if (IsEditorInstalled(version))
			return;

		if (string.IsNullOrEmpty(changeset))
		{
			AnsiConsole.MarkupLine("[cyan]Changeset not provided, fetching from Unity API...[/]");
			changeset = await UnityReleaseApi.FetchChangesetAsync(version);
		}

		InstallEditor(version, changeset);
	}
}