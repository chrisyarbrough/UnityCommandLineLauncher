using System.ComponentModel;

class InstallationsSettings : CommandSettings
{
	[CommandOption("--parseable")]
	[Description("Output in a simple machine-parseable format")]
	public bool Parseable { get; init; }
}
