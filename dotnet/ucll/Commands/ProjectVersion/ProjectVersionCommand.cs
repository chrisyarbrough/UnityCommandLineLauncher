class ProjectVersionCommand(UnityHub unityHub) : BaseCommand<ProjectVersionSettings>
{
	protected override int ExecuteImpl(ProjectVersionSettings settings)
	{
		string searchPath = settings.SearchPath ?? OpenCommand.PromptForRecentProject(settings.Favorite, unityHub);

		searchPath = Path.GetFullPath(searchPath);

		if (!Directory.Exists(searchPath) && !File.Exists(searchPath))
		{
			WriteError($"'{searchPath}' does not exist.");
			return 1;
		}

		UnityVersion info = ProjectVersionFile.Parse(searchPath, out string _);

		string output = info.Version;

		if (info.Changeset != null)
			output += " " + info.Changeset;

		AnsiConsole.WriteLine(output);
		return 0;
	}
}