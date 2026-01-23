using System.ComponentModel;

internal class ResetProjectSettings : MutatingSettings
{
	[CommandArgument(0, "[searchPath]")]
	[Description(Descriptions.SearchPath)]
	public string? SearchPath { get; init; }

	[CommandOption("-f|--favorite|--favorites")]
	[Description(Descriptions.Favorite)]
	public bool Favorite { get; init; }

	[CommandOption("--keep-user-settings")]
	[Description("Keep \"UserSettings\" folder")]
	public bool KeepUserSettings { get; init; }
}