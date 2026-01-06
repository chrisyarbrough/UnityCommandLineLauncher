class ProjectPathCommand : BaseCommand<ProjectPathSettings>
{
	protected override int ExecuteImpl(ProjectPathSettings settings)
	{
		var searchPath = settings.SearchPath ?? PromptForRecentProject(settings.Favorite);

		searchPath = Path.GetFullPath(searchPath);

		if (!Directory.Exists(searchPath) && !File.Exists(searchPath))
		{
			WriteError($"'{searchPath}' does not exist.");
			return 1;
		}

		string filePath = ProjectVersionFile.FindFilePath(searchPath);
		string projectDir = new FileInfo(filePath).Directory!.Parent!.FullName;

		AnsiConsole.WriteLine(projectDir);
		return 0;
	}

	private static string PromptForRecentProject(bool favoritesOnly)
	{
		var recentProjects = UnityHub.GetRecentProjects(favoritesOnly).ToArray();

		if (recentProjects.Length == 0)
			throw new Exception("No projects found in Unity Hub.");

		return SelectionPrompt.Prompt(
			recentProjects,
			$"Select a {(favoritesOnly ? "favorite" : "recent")} project: ");
	}
}
