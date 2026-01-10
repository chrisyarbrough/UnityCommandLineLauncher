internal class ProjectsUsingVersionCommand(PlatformSupport platformSupport) : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		foreach (string project in VersionUsage.FindUnityProjects(platformSupport)
			         .Where(path => Project.Parse(path).Version == settings.Version))
		{
			Console.WriteLine(project);
		}
		return 0;
	}
}