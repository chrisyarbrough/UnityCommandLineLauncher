using System.ComponentModel;

class UpmGitUrlSettings : CommandSettings
{
	[CommandArgument(0, "[searchPath]")]
	[Description(
		"Directory to be searched for a Unity project or path to ProjectVersion.txt (omit for interactive prompt)")]
	public string? SearchPath { get; init; }

	[CommandOption("-f|--favorite|--favorites")]
	[Description("Show only favorite projects in interactive selection")]
	public bool Favorite { get; init; }
}