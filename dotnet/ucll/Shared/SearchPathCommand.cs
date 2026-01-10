internal abstract class SearchPathCommand<TSettings>(UnityHub unityHub) : BaseCommand<TSettings>
	where TSettings : CommandSettings
{
	protected UnityHub UnityHub { get; } = unityHub;

	protected string ResolveSearchPath(string? searchPath, bool favoriteOnly)
	{
		string resolvedPath = searchPath ?? PromptForRecentProject(favoriteOnly, UnityHub);

		searchPath = Path.GetFullPath(resolvedPath);

		if (!Directory.Exists(searchPath) && !File.Exists(searchPath))
		{
			WriteError($"'{searchPath}' does not exist.");
			Environment.Exit(1);
		}
		return searchPath;
	}

	private static string PromptForRecentProject(bool favoriteOnly, UnityHub unityHub)
	{
		string[] recentProjects = unityHub.GetRecentProjects(favoriteOnly).ToArray();

		if (recentProjects.Length == 0)
			throw new UserException("No projects found in Unity Hub.");

		return SelectionPrompt.Prompt(
			recentProjects,
			$"Select a {(favoriteOnly ? "favorite" : "recent")} project: ");
	}
}