using System.ComponentModel;

class InstallSettings : MutatingCommand
{
	[CommandArgument(0, "<version>")]
	[Description("Unity version to install (e.g., 2022.3.10f1)")]
	public string Version { get; init; } = string.Empty;

	[CommandArgument(1, "[changeset]")]
	[Description("Unity changeset (optional, will be fetched from API if not provided)")]
	public string? Changeset { get; init; }
}