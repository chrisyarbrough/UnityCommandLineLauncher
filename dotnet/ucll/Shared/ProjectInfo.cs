internal record ProjectInfo(string Version, string? Changeset, string Path)
{
	public string VersionAndChangeset
		=> Changeset != null ? $"{Version} ({Changeset})" : Version;

	public override string ToString()
		=> $"{VersionAndChangeset}\n{Path}";
}