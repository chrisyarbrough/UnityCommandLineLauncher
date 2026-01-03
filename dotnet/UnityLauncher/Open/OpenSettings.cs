using System.ComponentModel;

class OpenSettings : MutatingCommand
{
	[CommandArgument(0, "[searchPath]")]
	[Description("Directory to be searched for a Unity project")]
	public string? SearchPath { get; init; }

	[Description("Additional arguments to pass to Unity Editor")]
	[CommandArgument(1, "[unityArgs]")]
	public string[]? UnityArgs { get; init; }
}