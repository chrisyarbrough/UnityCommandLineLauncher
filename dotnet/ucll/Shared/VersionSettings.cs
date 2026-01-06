using System.ComponentModel;

class VersionSettings : CommandSettings
{
	[CommandArgument(0, "<version>")]
	[Description("Unity version (e.g., 2022.3.10f1)")]
	public required string Version { get; init; }
}