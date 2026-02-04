internal class InstallCommand(UnityHub unityHub, PlatformSupport platformSupport) : BaseCommand<InstallSettings>
{
	protected override int ExecuteImpl(InstallSettings settings)
	{
		unityHub.InstallEditorChecked(
			settings.Version,
			settings.Changeset,
			platformSupport.CreateProcessRunner(settings.DryRun),
			Context.Remaining.Raw.ToArray());

		if (!settings.DryRun)
			WriteSuccess($"Unity {settings.Version} installed.");

		return 0;
	}
}