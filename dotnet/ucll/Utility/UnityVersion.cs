using System.Text.RegularExpressions;

/// <summary>
/// Follows the pattern: <code>Major.Minor.Patch[abfp]Revision</code>
/// For example: <i>2021.3.1f2</i> or <i>5.5.0p3</i>
/// </summary>
internal readonly struct UnityVersion : IComparable<UnityVersion>
{
	public int Major { get; init; }
	public int Minor { get; init; }
	public int Patch { get; init; }

	// Revisions can be alpha, beta, release candidate or legacy patch.
	private readonly char revisionType;

	public int Revision { get; init; }

	public UnityVersion(string version)
	{
		// Example: 						  2021 . 3    .  1     f     2
		Match match = Regex.Match(version, @"(\d+)\.(\d+)\.(\d+)([abfp])?(\d?)");
		GroupCollection groups = match.Groups;

		Major = int.Parse(groups[1].Value);

		if (groups[2].Length > 0)
			Minor = int.Parse(groups[2].Value);

		if (match.Groups[3].Length > 0)
			Patch = int.Parse(match.Groups[3].Value);

		if (match.Groups[4].Length > 0)
			revisionType = match.Groups[4].Value[0];

		if (match.Groups[5].Length > 0)
			Revision = int.Parse(match.Groups[5].Value);
	}

	public int CompareTo(UnityVersion other)
	{
		if (Major != other.Major)
			return Major.CompareTo(other.Major);

		if (Minor != other.Minor)
			return Minor.CompareTo(other.Minor);

		if (Patch != other.Patch)
			return Patch.CompareTo(other.Patch);

		return Revision.CompareTo(other.Revision);
	}

	public override string ToString()
	{
		return $"{Major}.{Minor}.{Patch}" + (Revision > 0 ? $"{revisionType}{Revision}" : "");
	}

	public static bool operator <(UnityVersion a, UnityVersion b) => a.CompareTo(b) < 0;

	public static bool operator >(UnityVersion a, UnityVersion b) => a.CompareTo(b) > 0;

	public static IEnumerable<string> SortNewestFirst(IEnumerable<string> unityVersions)
	{
		return unityVersions.OrderByDescending(v => new UnityVersion(v));
	}
}

internal class UnityVersionComparer : IComparer<string>
{
	public int Compare(string? x, string? y)
	{
		if (Comparer<string>.Default.Compare(x, y) == 0)
			return 0;

		return new UnityVersion(x!).CompareTo(new UnityVersion(y!));
	}
}