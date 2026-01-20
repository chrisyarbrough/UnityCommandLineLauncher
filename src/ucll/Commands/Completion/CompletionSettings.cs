using System.ComponentModel;

internal class CompletionSettings : CommandSettings
{
	[CommandArgument(0, "[shell]")]
	[Description("Shell type to generate completions for (currently only 'zsh' is supported)")]
	[DefaultValue("zsh")]
	public string Shell { get; init; } = "zsh";
}