class InstallCommand : BaseCommand<InstallSettings>
{
	protected override int ExecuteImpl(InstallSettings settings)
	{
		UnityHub.EnsureEditorInstalledAsync(settings.Version, settings.Changeset).Wait();
		ConsoleHelper.WriteSuccess($"Unity {settings.Version} is installed and ready to use.");
		return 0;
	}
}