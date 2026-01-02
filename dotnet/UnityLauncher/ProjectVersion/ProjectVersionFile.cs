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
		var foundFiles = Directory.EnumerateFiles(
				searchDir, "ProjectVersion.txt", SearchOption.AllDirectories)
			.Where(file => Path.GetFileName(Path.GetDirectoryName(file)) == "ProjectSettings").Take(8).ToArray();

		if (foundFiles.Length > 1)
		{
			throw new Exception(
				$"Found multiple ProjectVersion.txt files (showing partial results):\n{string.Join("\n", foundFiles)}\n" +
				"Specify a directory containing a single Unity project.");
		}
		else if (foundFiles.Length == 1)
		{
			return foundFiles[0];
		}

		return null;
	}

	[GeneratedRegex(@"m_EditorVersionWithRevision:\s+(.+)\s+\((.+)\)")]
	private static partial Regex EditorVersionWithRevisionRegex();

	[GeneratedRegex(@"m_EditorVersion:\s+(.+)")]
	private static partial Regex EditorVersionRegex();
}