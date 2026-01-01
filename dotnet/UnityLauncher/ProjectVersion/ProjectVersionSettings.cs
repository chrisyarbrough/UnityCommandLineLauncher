using System.ComponentModel;

class ProjectVersionSettings : CommandSettings
{
	[CommandArgument(0, "[searchDirectory]")]
	[Description("Directory to search for ProjectVersion.txt (default: current directory)")]
	public string SearchDirectory { get; init; } = ".";
}