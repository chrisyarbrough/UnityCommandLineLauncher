using System.ComponentModel;

class InstallationsOverviewSettings : CommandSettings
{
	[CommandOption("--parseable")]
	[Description("Output in a simple machine-parseable format")]
	public bool Parseable { get; init; }
}
