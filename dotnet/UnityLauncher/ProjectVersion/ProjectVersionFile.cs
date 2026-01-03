using System.Text.RegularExpressions;

record UnityVersion(string Version, string? Changeset)
{
	public override string ToString()
	{
		string s = $"Version: {Version}";
		if (Changeset != null)
			s += $" ({Changeset})";
		return s;
	}
}

static partial class ProjectVersionFile
{
	public static UnityVersion Parse(string directoryOrFile, out string filePath)
	{
		if (File.Exists(directoryOrFile))
			filePath = directoryOrFile;
		else if (Directory.Exists(directoryOrFile))
			filePath = Find(directoryOrFile);
		else
			throw new FileNotFoundException("Argument must be a valid directory or ProjectVersion.txt file path.");

		var content = File.ReadAllText(filePath);

		// Try pattern 1: m_EditorVersionWithRevision: 2021.3.45f1 (abc123)
		var match = EditorVersionWithRevisionRegex().Match(content);
		if (match.Success)
		{
			return new UnityVersion(
				Version: match.Groups[1].Value,
				Changeset: match.Groups[2].Value
			);
		}

		// Try pattern 2: m_EditorVersion: 2021.3.45f1
		match = EditorVersionRegex().Match(content);
		if (match.Success)
		{
			return new UnityVersion(
				Version: match.Groups[1].Value,
				Changeset: null
			);
		}

		throw new Exception($"Could not find Unity version in {filePath}");
	}

	private static string Find(string searchDir)
	{
		var upwardResult = FindUpward(searchDir);
		if (upwardResult != null)
			return upwardResult;

		var downwardResult = FindDownward(searchDir);
		if (downwardResult == null)
			throw new Exception($"No ProjectSettings/ProjectVersion.txt found in {searchDir}");
		return downwardResult;
	}

	private static string? FindUpward(string startDir)
	{
		using var _ = new ProfilingTimer("Find Upward");
		var current = new DirectoryInfo(startDir);

		while (current != null)
		{
			var projectSettingsDir = Path.Combine(current.FullName, "ProjectSettings");

			if (Directory.Exists(projectSettingsDir))
			{
				var versionFilePath = Path.Combine(projectSettingsDir, "ProjectVersion.txt");

				if (File.Exists(versionFilePath))
					return versionFilePath;
			}

			current = current.Parent;
		}

		return null;
	}

	private static string? FindDownward(string searchDir)
	{
		var ignoredFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			".git", "Library", "Temp", "Logs", "obj", "bin",
			"Packages", "UserSettings", "Build", "Builds", "node_modules",
		};

		var foundFiles = new List<string>();
		using (new ProfilingTimer("Find Downward"))
		{
			SearchRecursive(new DirectoryInfo(searchDir), ignoredFolders, foundFiles);
		}

		if (foundFiles.Count > 1)
		{
			var selection = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
					.Title($"Found {foundFiles.Count} Unity projects. [green]Select one[/]:")
					.PageSize(10)
					.EnableSearch()
					.AddChoices(foundFiles.OrderBy(x => x)));

			return selection;
		}
		else if (foundFiles.Count == 1)
		{
			return foundFiles[0];
		}

		return null;
	}

	private static void SearchRecursive(DirectoryInfo currentDir, HashSet<string> ignoredFolders, List<string> results)
	{
		try
		{
			var versionFile = Path.Combine(currentDir.FullName, "ProjectSettings", "ProjectVersion.txt");
			if (File.Exists(versionFile))
			{
				results.Add(versionFile);

				// Unity projects cannot be nested - no need to search deeper
				return;
			}

			foreach (var subDir in currentDir.EnumerateDirectories())
			{
				if (ignoredFolders.Contains(subDir.Name))
					continue;

				SearchRecursive(subDir, ignoredFolders, results);
			}
		}
		catch (Exception)
		{
			// Skip directories we don't have permission to access or that were deleted while searching.
		}
	}

	[GeneratedRegex(@"m_EditorVersionWithRevision:\s+(.+)\s+\((.+)\)")]
	private static partial Regex EditorVersionWithRevisionRegex();

	[GeneratedRegex(@"m_EditorVersion:\s+(.+)")]
	private static partial Regex EditorVersionRegex();
}