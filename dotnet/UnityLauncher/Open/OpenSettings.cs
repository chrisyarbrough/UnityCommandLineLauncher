using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)]
class OpenSettings : CommandSettings
{
	[CommandArgument(0, "<projectPath>")]
	[Description("Path to Unity project directory (default: current directory)")]
	public required string ProjectPath { get; init; }

	[Description("Additional arguments to pass to Unity Editor")]
	[CommandArgument(1, "[unityArgs]")]
	public string[]? UnityArgs { get; init; }
}