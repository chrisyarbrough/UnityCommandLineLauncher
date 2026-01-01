using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)]

class GetRevisionSettings : CommandSettings
{
	[CommandArgument(0, "<version>")]
	[Description("Unity version to get the revision for (e.g., 2022.3.10f1)")]
	public string Version { get; set; } = string.Empty;
}
