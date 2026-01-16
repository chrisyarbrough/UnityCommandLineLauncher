internal class ProjectVersionCommand(UnityHub unityHub) : SearchPathCommand<ProjectVersionSettings>(unityHub)
{
	protected override int ExecuteImpl(ProjectVersionSettings settings)
	{
		string searchPath = ResolveSearchPath(settings.SearchPath, settings.Favorite);

		ProjectInfo info = Project.Parse(searchPath);

		string output = info.Version;

		if (info.Changeset != null)
			output += " " + info.Changeset;

		Console.WriteLine(output);
		return 0;
	}
}