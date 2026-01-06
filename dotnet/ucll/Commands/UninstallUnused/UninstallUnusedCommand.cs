class UninstallUnusedCommand(PlatformSupport platformSupport, UnityHub unityHub)
	: BaseCommand<MutatingCommand>
{
	protected override int ExecuteImpl(MutatingCommand settings)
	{
		var versions = new VersionUsage(platformSupport, unityHub);
		var unusedVersions = versions.InstalledNotUsed.ToList();

		if (unusedVersions.Count == 0)
		{
			WriteSuccess("All installed Unity versions are being used.");
			return 0;
		}

		AnsiConsole.MarkupLine($"[bold]Found {unusedVersions.Count} unused Unity version(s):[/]");
		AnsiConsole.WriteLine();

		var versionPaths = new Dictionary<string, string>();
		foreach (string version in unusedVersions.OrderBy(v => v))
		{
			try
			{
				string editorPath = unityHub.GetEditorPath(version);
				string installDir = platformSupport.FindInstallationRoot(editorPath);
				versionPaths[version] = installDir;

				AnsiConsole.MarkupLine($"  [yellow]•[/] {Markup.Escape(version)}");
				AnsiConsole.MarkupLine($"    [dim]{Markup.Escape(installDir)}[/]");
			}
			catch (Exception ex)
			{
				AnsiConsole.MarkupLine(
					$"  [red]•[/] {Markup.Escape(version)} [dim](Error: {Markup.Escape(ex.Message)})[/]");
			}
		}

		if (versionPaths.Count == 0)
		{
			WriteError("No valid installation directories found.");
			return 1;
		}

		AnsiConsole.WriteLine();

		bool shouldUninstall = AnsiConsole.Confirm(
			"Do you want to uninstall these versions?",
			defaultValue: false);

		if (!shouldUninstall)
		{
			AnsiConsole.MarkupLine("[yellow]Uninstallation cancelled.[/]");
			return 0;
		}

		AnsiConsole.WriteLine();

		foreach (var (version, installDir) in versionPaths)
		{
			AnsiConsole.MarkupLine($"[bold]Uninstalling Unity {Markup.Escape(version)}...[/]");

			try
			{
				if (settings.DryRun)
				{
					AnsiConsole.MarkupLine($"[dim]Would delete: {Markup.Escape(installDir)}[/]");
				}
				else
				{
					Directory.Delete(installDir, recursive: true);
				}

				WriteSuccess($"Unity {version} uninstalled successfully.");
			}
			catch (Exception ex)
			{
				WriteError($"Failed to uninstall Unity {version}: {ex.Message}");
			}

			AnsiConsole.WriteLine();
		}

		WriteSuccess("Uninstallation process completed.");
		return 0;
	}
}