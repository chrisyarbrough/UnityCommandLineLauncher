using System.ComponentModel;

internal class CreateSettings : MutatingSettings
{
	[CommandArgument(0, "<path>")]
	[Description("Path where the Unity project should be created")]
	public string ProjectPath { get; init; } = string.Empty;

	[CommandArgument(1, "[version]")]
	[Description("Unity version (e.g., 2022.3.10f1)")]
	public string? Version { get; init; }

	[CommandOption("-m|--minimal")]
	[Description("Creates a bare-minimum project (fast)")]
	public bool Minimal { get; init; }
}