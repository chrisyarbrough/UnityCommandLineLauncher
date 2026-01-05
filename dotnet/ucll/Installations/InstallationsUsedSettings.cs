using System.ComponentModel;

class InstallationsUsedSettings : CommandSettings
{
	[CommandArgument(0, "<version>")]
	[Description("Unity version to search for (e.g., 2022.3.10f1)")]
	public required string Version { get; init; }
}
