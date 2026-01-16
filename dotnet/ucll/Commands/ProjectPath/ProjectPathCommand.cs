internal class ProjectPathCommand(UnityHub unityHub) : SearchPathCommand<ProjectPathSettings>(unityHub)
{
	protected override int ExecuteImpl(ProjectPathSettings settings)
	{
		string searchPath = ResolveSearchPath(settings.SearchPath, settings.Favorite);
		string projectPath = Project.FindProjectPath(searchPath);
		Console.WriteLine(projectPath);
		return 0;
	}
}