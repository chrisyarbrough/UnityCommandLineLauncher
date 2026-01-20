using System.ComponentModel;

internal class MutatingSettings : CommandSettings
{
	[CommandOption("--dry-run")]
	[Description("Show what would be executed without actually running mutating commands.")]
	public bool DryRun { get; init; }

	[CommandOption("-y|--yes")]
	[Description("Skip interactive confirmation prompts and assume yes.")]
	public bool Yes { get; init; }

	public IProcessRunner MutatingProcess => DryRun ? DryRunProcessRunner.DryRun : ProcessRunner.Default;
}