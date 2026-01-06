class InstallCommand(UnityHub unityHub) : BaseCommand<InstallSettings>
{
	protected override int ExecuteImpl(InstallSettings settings)
	{
		string[] additionalArgs = context.Remaining.Raw.ToArray();
		unityHub.InstallEditorChecked(settings.Version, settings.Changeset, settings.MutatingProcess, additionalArgs);

		WriteSuccess($"Unity {settings.Version} is installed.");
		return 0;
	}
}