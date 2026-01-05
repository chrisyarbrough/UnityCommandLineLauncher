using System.ComponentModel;

class InstallationsOverviewSettings : CommandSettings
{
	[CommandOption("-p|--plaintext|--plain")]
	[Description("Output in a simple machine-parseable format")]
	public bool PlainText { get; init; }
}
