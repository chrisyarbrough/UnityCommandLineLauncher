class InstallCommand : BaseCommand<InstallSettings>
{
	protected override int ExecuteImpl(InstallSettings settings)
	{
		new UnityHub(settings.MutatingProcess)
			.EnsureEditorInstalledAsync(settings.Version, settings.Changeset).Wait();

		WriteSuccess($"Unity {settings.Version} is installed.");
		return 0;
	}
}