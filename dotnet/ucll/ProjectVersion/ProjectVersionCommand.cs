class ProjectVersionCommand : BaseCommand<ProjectVersionSettings>
{
	protected override int ExecuteImpl(ProjectVersionSettings settings)
	{
		var versionInfo = ProjectVersionFile.Parse(settings.SearchPath, out string _);
		string output = versionInfo.Version;

		if (versionInfo.Changeset != null)
			output += " " + versionInfo.Changeset;

		AnsiConsole.WriteLine(output);
		return 0;
	}
}