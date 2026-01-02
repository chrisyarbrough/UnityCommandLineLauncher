using System.ComponentModel;

class MutatingCommand : CommandSettings
{
	[CommandOption("--dry-run")]
	[Description("Show what would be executed without actually running modifying commands.")]
	public bool DryRun { get; init; }

	public IProcessRunner MutatingProcess => DryRun ? new DryRunProcessRunner() : new ProcessRunner();
}