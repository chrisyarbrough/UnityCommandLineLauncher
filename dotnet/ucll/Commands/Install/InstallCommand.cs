class InstallCommand : BaseCommand<InstallSettings>
{
	protected override int ExecuteImpl(InstallSettings settings)
	{
		string[] additionalArgs = context.Remaining.Raw.ToArray();
		new UnityHub(settings.MutatingProcess)
			.EnsureEditorInstalled(settings.Version, settings.Changeset, additionalArgs);

		WriteSuccess($"Unity {settings.Version} is installed.");
		return 0;
	}
}