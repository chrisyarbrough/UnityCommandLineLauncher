class InstallationsUsedCommand : BaseCommand<InstallationsUsedSettings>
{
	protected override int ExecuteImpl(InstallationsUsedSettings settings)
	{
		foreach (string project in VersionUsage.FindUnityProjects()
			         .Where(path => ProjectVersionFile.Parse(path).Version == settings.Version))
		{
			Console.WriteLine(project);
		}
		return 0;
	}
}