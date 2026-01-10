internal class ProjectVersionCommand(UnityHub unityHub) : SearchPathCommand<ProjectVersionSettings>(unityHub)
{
	protected override int ExecuteImpl(ProjectVersionSettings settings)
	{
		string searchPath = ResolveSearchPath(settings.SearchPath, settings.Favorite);

		UnityVersion info = ProjectVersionFile.Parse(searchPath, out string _);

		string output = info.Version;

		if (info.Changeset != null)
			output += " " + info.Changeset;

		AnsiConsole.WriteLine(output);
		return 0;
	}
}