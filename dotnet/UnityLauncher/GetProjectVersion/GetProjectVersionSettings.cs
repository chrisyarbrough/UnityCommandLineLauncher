using System.ComponentModel;

class GetProjectVersionSettings : CommandSettings
{
	[CommandArgument(0, "<project-version-file-path>")]
	[Description("Path to ProjectVersion.txt file")]
	public string ProjectVersionFilePath { get; init; } = string.Empty;
}