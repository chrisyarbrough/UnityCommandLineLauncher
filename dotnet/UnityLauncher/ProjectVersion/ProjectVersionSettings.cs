using System.ComponentModel;

class ProjectVersionSettings : CommandSettings
{
	[CommandArgument(0, "<searchPath>")]
	[Description("Directory to be searched for a Unity project")]
	public required string SearchPath { get; init; }
}