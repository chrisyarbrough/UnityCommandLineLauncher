using System.Text.RegularExpressions;

record UnityVersion(string Version, string? Changeset);

static partial class ProjectVersionFile
{
	public static UnityVersion Parse(string filePath)
	{
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

	[GeneratedRegex(@"m_EditorVersionWithRevision:\s+(.+)\s+\((.+)\)")]
	private static partial Regex EditorVersionWithRevisionRegex();

	[GeneratedRegex(@"m_EditorVersion:\s+(.+)")]
	private static partial Regex EditorVersionRegex();
}