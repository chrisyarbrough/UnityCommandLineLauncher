internal class ResetProjectCommand(UnityHub unityHub) : SearchPathCommand<ResetProjectSettings>(unityHub)
{
	/// <summary>
	/// A collection of path components to match against.
	/// </summary>
	private class PathComponents() : HashSet<string>(StringComparer.OrdinalIgnoreCase);

	protected override int ExecuteImpl(ResetProjectSettings settings)
	{
		string searchPath = ResolveSearchPath(settings.SearchPath, settings.Favorite);
		var project = Project.Parse(searchPath);

		var targetDirs = new PathComponents
		{
			"Library",
			"obj",
			"Logs",
			"Temp",
			".vs",
			".idea",
			".utmp",
		};

		if (!settings.KeepUserSettings)
			targetDirs.Add("UserSettings");

		var targetFileExtensions = new PathComponents
		{
			".csproj",
			".sln",
			".user",
			".vsconfig",
		};

		var targetFilePartialNames = new PathComponents
		{
			"InitTestScene",
			"mono_crash",
		};

		PromptForConfirmation(settings);

		int deletedDirs = 0;
		int deletedFiles = 0;

		Action<string, string> processor;

		if (settings.DryRun)
		{
			processor = (_, label) => AnsiConsole.MarkupLine($"[dim]Would delete: {label}[/]");
		}
		else
		{
			processor = (path, label) =>
			{
				if (Directory.Exists(path))
				{
					Directory.Delete(path, true);
					deletedDirs++;
				}
				else if (File.Exists(path))
				{
					File.Delete(path);
					deletedFiles++;
				}
				AnsiConsole.WriteLine($"Deleted: {label}");
			};
		}

		foreach (string path in Directory.GetFileSystemEntries(project.Path))
		{
			string entryName = Path.GetFileName(path);

			if (!targetDirs.Contains(entryName) &&
			    !targetFileExtensions.Contains(Path.GetExtension(entryName)) &&
			    !targetFilePartialNames.Any(p => path.Contains(p)))
			{
				continue;
			}

			processor.Invoke(path, Markup.Escape(entryName));
		}

		WriteSuccess($"Deleted {deletedDirs} directories and {deletedFiles} files the project.");
		return 0;
	}

	private static void PromptForConfirmation(ResetProjectSettings settings)
	{
		if (settings.Yes)
			return;

		if (!AnsiConsole.Confirm("Do you want to reset this project?", defaultValue: false))
		{
			throw new UserCancelledException("Resetting cancelled.");
		}
	}
}