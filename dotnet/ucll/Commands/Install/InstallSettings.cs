using System.ComponentModel;

internal class InstallSettings : MutatingSettings
{
	[CommandArgument(0, "<version>")]
	[Description(Descriptions.Version)]
	public string Version { get; init; } = string.Empty;

	[CommandArgument(1, "[changeset]")]
	[Description("Unity changeset (optional, will be fetched from API if not provided)")]
	public string? Changeset { get; init; }
}