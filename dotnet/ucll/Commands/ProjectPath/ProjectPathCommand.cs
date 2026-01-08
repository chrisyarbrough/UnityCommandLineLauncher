internal class ProjectPathCommand(UnityHub unityHub) : BaseCommand<ProjectPathSettings>
{
	protected override int ExecuteImpl(ProjectPathSettings settings)
	{
		string searchPath = settings.SearchPath ?? OpenCommand.PromptForRecentProject(settings.Favorite, unityHub);

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
}