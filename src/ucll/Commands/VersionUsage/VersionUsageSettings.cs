using System.ComponentModel;

internal class VersionUsageSettings : CommandSettings
{
	[CommandOption("-p|--plaintext|--plain")]
	[Description("Output in a simple machine-parseable format")]
	public bool PlainText { get; init; }

	[CommandOption("-m|--modules")]
	[Description("Include installed modules for each editor version")]
	public bool Modules { get; init; }
}