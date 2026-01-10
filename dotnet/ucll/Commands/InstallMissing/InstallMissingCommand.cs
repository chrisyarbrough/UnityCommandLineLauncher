internal class InstallMissingCommand(PlatformSupport platformSupport, UnityHub unityHub)
	: BaseCommand<MutatingSettings>
{
	protected override int ExecuteImpl(MutatingSettings settings)
	{
		var versions = new VersionUsage(platformSupport, unityHub);
		var missingVersions = versions.UsedNotInstalled.ToList();

		if (missingVersions.Count == 0)
		{
			WriteSuccess("All used Unity versions are already installed.");
			return 0;
		}

		AnsiConsole.MarkupLine($"[bold]Found {missingVersions.Count} missing Unity version(s):[/]");
		AnsiConsole.WriteLine();

		foreach (string version in missingVersions)
		{
			AnsiConsole.MarkupLine($"  [yellow]•[/] {Markup.Escape(version)}");
		}

		AnsiConsole.WriteLine();

		bool shouldInstall = AnsiConsole.Confirm(
			"Do you want to install these versions?",
			defaultValue: false);

		if (!shouldInstall)
		{
			AnsiConsole.MarkupLine("[yellow]Installation cancelled.[/]");
			return 0;
		}

		AnsiConsole.WriteLine();

		foreach (string version in missingVersions)
		{
			AnsiConsole.MarkupLine($"[bold]Installing Unity {Markup.Escape(version)}...[/]");

			try
			{
				string[] additionalArgs = Context.Remaining.Raw.ToArray();
				unityHub.InstallEditor(version, changeset: null, settings.MutatingProcess, additionalArgs);
				WriteSuccess($"Unity {version} installed successfully.");
			}
			catch (Exception ex)
			{
				WriteError($"Failed to install Unity {version}: {ex.Message}");
			}

			AnsiConsole.WriteLine();
		}

		WriteSuccess("Installation process completed.");
		return 0;
	}
}