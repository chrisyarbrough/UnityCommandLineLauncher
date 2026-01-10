using System.ComponentModel;

internal class OpenSettings : MutatingSettings
{
	[CommandArgument(0, "[searchPath]")]
	[Description(
		"Directory to be searched for a Unity project or path to ProjectVersion.txt (omit for interactive prompt")]
	public string? SearchPath { get; init; }

	[CommandOption("-f|--favorite|--favorites")]
	[Description("Show only favorite projects in interactive selection")]
	public bool Favorite { get; init; }

	[CommandOption("-c|--code-editor")]
	[Description("Open the solution file in the default code editor")]
	public bool CodeEditor { get; init; }
}