internal class ResetProjectCommand(UnityHub unityHub) : SearchPathCommand<ResetProjectSettings>(unityHub)
{
	protected override int ExecuteImpl(ResetProjectSettings settings)
	{
		string searchPath = ResolveSearchPath(settings.SearchPath, settings.Favorite);
		ProjectInfo info = Project.Parse(searchPath);

		var resetTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Assets", "Packages", "ProjectSettings",
		};

		bool shouldReset = settings.Yes || AnsiConsole.Confirm(
			"Do you want to reset this project?",
			defaultValue: false);

		if (!shouldReset)
		{
			throw new UserCancelledException("Resetting cancelled.");
		}

		AnsiConsole.WriteLine();

		try
		{
			foreach (string item in Directory.GetFileSystemEntries(info.Path))
			{
				string itemName = Path.GetFileName(item);

				if (resetTargets.Contains(itemName)) continue;

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

						WriteSuccess($"Directory {Markup.Escape(item)} deleted successfully.");
					}
					else if (File.Exists(item))
					{
						AnsiConsole.MarkupLine($"[bold]Deleting file {Markup.Escape(itemName)}...[/]");

						File.Delete(item);

						WriteSuccess($"File {Markup.Escape(itemName)} deleted successfully.");
					}
				}
			}
		}
		catch (Exception ex)
		{
			WriteError($"Failed to reset this project: {ex.Message}");
		}

		WriteSuccess("Resetting process completed.");
		return 0;
	}
}