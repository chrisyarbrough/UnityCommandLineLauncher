using System.Text.RegularExpressions;

record UnityVersion(string Version, string? Changeset);

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
		// Try upward search first (find closest containing project)
		var upwardResult = FindUpward(searchDir);
		if (upwardResult != null)
			return upwardResult;

		// Fall back to downward search if upward didn't find anything
		return FindDownward(searchDir);
	}

	private static string? FindUpward(string startDir)
	{
		var current = new DirectoryInfo(startDir);

		// Search upward until we reach the filesystem root
		while (current != null)
		{
			var projectSettingsDir = Path.Combine(current.FullName, "ProjectSettings");

			if (Directory.Exists(projectSettingsDir))
			{
				var versionFilePath = Path.Combine(projectSettingsDir, "ProjectVersion.txt");

				// If ProjectSettings exists and contains ProjectVersion.txt, we found it
				if (File.Exists(versionFilePath))
					return versionFilePath;

				// If ProjectSettings exists but no ProjectVersion.txt, continue searching
				// (could be a user's custom ProjectSettings folder, not Unity's)
			}

			// Move to parent directory
			current = current.Parent;
		}

		// Reached filesystem root without finding anything
		return null;
	}

	private static string FindDownward(string searchDir)
	{
		var foundFiles = new List<string>();

		// Enumerate files and abort early if we find more than one
		foreach (var file in Directory.EnumerateFiles(searchDir, "ProjectVersion.txt", SearchOption.AllDirectories))
		{
			// Only consider files in ProjectSettings folders
			if (Path.GetFileName(Path.GetDirectoryName(file)) == "ProjectSettings")
			{
				foundFiles.Add(file);

				// Abort enumeration as soon as we find multiple projects
				if (foundFiles.Count > 1)
				{
					throw new Exception(
						$"Found multiple ProjectVersion.txt files:\n{string.Join("\n", foundFiles)}\n" +
						"Please run in a directory with only one Unity project");
				}
			}
		}

		if (foundFiles.Count == 0)
			throw new Exception($"No ProjectSettings/ProjectVersion.txt found in {searchDir}");

		return foundFiles[0];
	}

	[GeneratedRegex(@"m_EditorVersionWithRevision:\s+(.+)\s+\((.+)\)")]
	private static partial Regex EditorVersionWithRevisionRegex();

	[GeneratedRegex(@"m_EditorVersion:\s+(.+)")]
	private static partial Regex EditorVersionRegex();
}