using System.Text.RegularExpressions;

static partial class ProjectVersionFile
{
	public static UnityVersion Parse(string directoryOrFile) => Parse(directoryOrFile, out string _);

	public static UnityVersion Parse(string directoryOrFile, out string filePath)
	{
		filePath = FindFilePath(directoryOrFile);
		string content = File.ReadAllText(filePath);

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

		throw new UserException($"Could not find Unity version in: {filePath}");
	}

	public static string FindFilePath(string directoryOrFile)
	{
		if (File.Exists(directoryOrFile))
		{
			if (directoryOrFile.EndsWith("ProjectVersion.txt"))
			{
				return directoryOrFile;
			}
			else
			{
				// This would be more or less unintended use: a file that is not the version file.
				return Find(Path.GetDirectoryName(directoryOrFile)!);
			}
		}
		else if (Directory.Exists(directoryOrFile))
		{
			return Find(directoryOrFile);
		}
		else
		{
			throw new UserException("Argument must be a valid directory or ProjectVersion.txt file path.");
		}
	}

	private static string Find(string searchDir)
	{
		foreach (var strategy in (Func<string, string?>[])[FindUpward, FindDownward])
		{
			string? result = strategy.Invoke(searchDir);
			if (result != null)
				return result;
		}
		throw new UserException($"No ProjectSettings/ProjectVersion.txt found in {searchDir}");
	}

	private static string? FindUpward(string startDir)
	{
		using var _ = new ProfilingTimer("Find Upward");
		var current = new DirectoryInfo(startDir);

		while (current != null)
		{
			string projectSettingsDir = Path.Combine(current.FullName, "ProjectSettings");

			if (Directory.Exists(projectSettingsDir))
			{
				string versionFilePath = Path.Combine(projectSettingsDir, "ProjectVersion.txt");

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
			".git", "Library", "Temp", "Logs", "obj", "bin", "Assets",
			"Packages", "UserSettings", "Build", "Builds", "node_modules",
		};

		var foundFiles = new List<string>();
		using (new ProfilingTimer("Find Downward"))
		{
			SearchRecursive(new DirectoryInfo(searchDir), ignoredFolders, foundFiles);
		}

		if (foundFiles.Count > 1)
		{
			foundFiles.Sort();
			return SelectionPrompt.Prompt(foundFiles, "Select one of the found projects: ");
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
			string versionFile = Path.Combine(currentDir.FullName, "ProjectSettings", "ProjectVersion.txt");
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