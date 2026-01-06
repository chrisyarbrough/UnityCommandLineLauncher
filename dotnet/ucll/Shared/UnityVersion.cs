record UnityVersion(string Version, string? Changeset)
{
	public override string ToString()
	{
		string s = Version;
		if (Changeset != null)
			s += $" ({Changeset})";
		return s;
	}
}