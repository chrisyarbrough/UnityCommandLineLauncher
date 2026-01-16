internal class InstallCommand(UnityHub unityHub) : BaseCommand<InstallSettings>
{
	protected override int ExecuteImpl(InstallSettings settings)
	{
		string[] additionalArgs = Context.Remaining.Raw.ToArray();
		unityHub.InstallEditorChecked(settings.Version, settings.Changeset, settings.MutatingProcess, additionalArgs);
		if (!settings.DryRun)
			WriteSuccess($"Unity {settings.Version} installed.");
		return 0;
	}
}