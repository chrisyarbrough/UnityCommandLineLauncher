using System.ComponentModel;

internal class MutatingCommand : CommandSettings
{
	[CommandOption("--dry-run")]
	[Description("Show what would be executed without actually running mutating commands.")]
	public bool DryRun { get; init; }

	public IProcessRunner MutatingProcess => DryRun ? DryRunProcessRunner.DryRun : ProcessRunner.Default;
}