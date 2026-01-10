internal class ProjectPathCommand(UnityHub unityHub) : SearchPathCommand<ProjectPathSettings>(unityHub)
{
	protected override int ExecuteImpl(ProjectPathSettings settings)
	{
		string searchPath = ResolveSearchPath(settings.SearchPath, settings.Favorite);

		string filePath = ProjectVersionFile.FindFilePath(searchPath);
		string projectDir = new FileInfo(filePath).Directory!.Parent!.FullName;

		AnsiConsole.WriteLine(projectDir);
		return 0;
	}
}