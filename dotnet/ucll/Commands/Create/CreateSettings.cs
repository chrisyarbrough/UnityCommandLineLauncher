using System.ComponentModel;

class CreateSettings : MutatingCommand
{
	[CommandArgument(0, "<path>")]
	[Description("Path where the Unity project should be created")]
	public string ProjectPath { get; init; } = string.Empty;

	[CommandArgument(1, "<version>")]
	[Description("Unity version to use (e.g., 2022.3.10f1)")]
	public string Version { get; init; } = string.Empty;
}