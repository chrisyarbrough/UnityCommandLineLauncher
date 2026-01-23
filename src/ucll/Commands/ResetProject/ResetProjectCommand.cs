internal class ResetProjectCommand(UnityHub unityHub) : SearchPathCommand<ResetProjectSettings>(unityHub)
{
	protected override int ExecuteImpl(ResetProjectSettings settings)
	{
		string searchPath = ResolveSearchPath(settings.SearchPath, settings.Favorite);
		var info = Project.Parse(searchPath);

		var targetDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Library",
			"obj",
			"Logs",
			"Temp",
			".vs",
			".idea",
		};

		if (!settings.KeepUserSettings)
			targetDirs.Add("UserSettings");

		var targetFilesExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			".csproj",
			".sln",
			".user",
			".vsconfig",
		};

		bool shouldReset = settings.Yes || AnsiConsole.Confirm(
			"Do you want to reset this project?",
			defaultValue: false);

		if (!shouldReset)
			throw new UserCancelledException("Resetting cancelled.");

		AnsiConsole.WriteLine();

		int deletedDirs = 0;
		int deletedFiles = 0;

		try
		{
			foreach (string item in Directory.GetFileSystemEntries(info.Path))
			{
				string itemName = Path.GetFileName(item);

				if (!targetDirs.Contains(itemName) && !targetFilesExtensions.Contains(Path.GetExtension(itemName)))
					continue;

				if (settings.DryRun)
				{
					AnsiConsole.MarkupLine($"[dim]Would delete: {Markup.Escape(itemName)}[/]");
				}
				else
				{
					if (Directory.Exists(item))
					{
						AnsiConsole.MarkupLine($"[bold]Deleting directory {Markup.Escape(itemName)}...[/]");

						Directory.Delete(item, true);

						WriteSuccess($"Directory {Markup.Escape(itemName)} deleted successfully.");
						deletedDirs++;
					}
					else if (File.Exists(item))
					{
						AnsiConsole.MarkupLine($"[bold]Deleting file {Markup.Escape(itemName)}...[/]");

						File.Delete(item);

						WriteSuccess($"File {Markup.Escape(itemName)} deleted successfully.");
						deletedFiles++;
					}
				}
			}
		}
		catch (Exception ex)
		{
			WriteError($"Failed to reset this project: {ex.Message}");
		}

		WriteSuccess("Resetting process completed.");
		WriteSuccess($"Deleted {deletedDirs} directories and {deletedFiles} files in project root.");
		return 0;
	}
}