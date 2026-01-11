internal class ProjectsUsingVersionCommand(PlatformSupport platformSupport, UnityHub unityHub) : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		string version = settings.GetVersionOrPrompt(unityHub);
		foreach (string project in VersionUsage.FindUnityProjects(platformSupport)
			         .Where(path => Project.Parse(path).Version == version))
		{
			Console.WriteLine(project);
		}
		return 0;
	}
}