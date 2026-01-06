class InstallationsUsedCommand : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		foreach (string project in VersionUsage.FindUnityProjects()
			         .Where(path => ProjectVersionFile.Parse(path).Version == settings.Version))
		{
			Console.WriteLine(project);
		}
		return 0;
	}
}