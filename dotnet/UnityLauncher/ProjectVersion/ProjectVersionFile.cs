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
		var foundFiles = Directory.EnumerateFiles(
				searchDir, "ProjectVersion.txt", SearchOption.AllDirectories)
			.Where(file => Path.GetFileName(Path.GetDirectoryName(file)) == "ProjectSettings").ToArray();

		if (foundFiles.Length == 0)
			throw new Exception($"No ProjectSettings/ProjectVersion.txt found in {searchDir}");

		if (foundFiles.Length > 1)
		{
			throw new Exception(
				$"Found multiple ProjectVersion.txt files:\n{string.Join("\n", foundFiles)}\n" +
				"Please run in a directory with only one Unity project");
		}

		return foundFiles[0];
	}

	[GeneratedRegex(@"m_EditorVersionWithRevision:\s+(.+)\s+\((.+)\)")]
	private static partial Regex EditorVersionWithRevisionRegex();

	[GeneratedRegex(@"m_EditorVersion:\s+(.+)")]
	private static partial Regex EditorVersionRegex();
}