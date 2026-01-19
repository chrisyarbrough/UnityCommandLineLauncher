internal class VersionUsageCommand(PlatformSupport platformSupport, UnityHub unityHub)
	: BaseCommand<VersionUsageSettings>
{
	protected override int ExecuteImpl(VersionUsageSettings settings)
	{
		var versions = new VersionUsage(platformSupport, unityHub);

		if (settings.PlainText)
			PrintPlainText(versions, settings.Modules);
		else
			PrintTable(versions, settings.Modules);

		return 0;
	}

	private static void PrintTable(VersionUsage versionUsage, bool includeModules)
	{
		var table = new Table();
		table.Border(TableBorder.Rounded);

		List<string> columnHeaders =
		[
			"Version",
			"Installed",
			"Used",
		];

		Dictionary<string, IEnumerable<string>> modules = new();
		if (includeModules)
		{
			HashSet<string> uniqueModulesNames = [];
			foreach (string version in UnityVersion.SortNewestFirst(versionUsage.Installed.Union(versionUsage.Used)))
			{
				modules[version] = versionUsage.GetInstalledModules(version);
				foreach (string module in modules[version])
					uniqueModulesNames.Add(module);
			}
			columnHeaders.AddRange(uniqueModulesNames);
		}

		foreach (string module in columnHeaders)
			table.AddColumn($"[bold]{module}[/]");

		foreach (string version in UnityVersion.SortNewestFirst(versionUsage.Installed.Union(versionUsage.Used)))
		{
			bool isInstalled = versionUsage.Installed.Contains(version);
			bool isUsed = versionUsage.Used.Contains(version);

			static string GetIcon(bool value) => value ? "[green]✓[/]" : "[red]✗[/]";

			string[] row = columnHeaders.ToArray();
			row[0] = Markup.Escape(version);
			row[1] = GetIcon(isInstalled);
			row[2] = GetIcon(isUsed);
			if (includeModules)
			{
				string[] modulesForRow = modules[version].ToArray();
				for (int i = 3; i < row.Length; i++)
				{
					row[i] = GetIcon(modulesForRow.Contains(row[i]));
				}
			}

			table.AddRow(row);
		}

		AnsiConsole.Write(table);

		AnsiConsole.WriteLine();
		AnsiConsole.MarkupLine("[bold]Summary:[/]");
		AnsiConsole.MarkupLine($"  Installed versions: [cyan]{versionUsage.Installed.Count}[/]");
		AnsiConsole.MarkupLine($"  Used versions: [cyan]{versionUsage.Used.Count}[/]");
		AnsiConsole.MarkupLine($"  Used but not installed: [yellow]{versionUsage.UsedNotInstalled.Count}[/]");
		AnsiConsole.MarkupLine($"  Installed but not used: [yellow]{versionUsage.InstalledNotUsed.Count}[/]");
	}

	private static void PrintPlainText(VersionUsage usage, bool includeModules)
	{
		void PrintVersions(string header, IEnumerable<string> versions)
		{
			Console.WriteLine(header);
			foreach (string version in UnityVersion.SortNewestFirst(versions))
			{
				if (includeModules)
				{
					string[] modules = usage.GetInstalledModules(version).ToArray();
					if (modules.Length == 0)
						Console.WriteLine(version);

					const string separator = "  ";
					Console.WriteLine(version + separator + string.Join(separator, modules));
				}
				else
				{
					Console.WriteLine(version);
				}
			}
		}

		PrintVersions("# Installed: " + usage.Installed.Count, usage.Installed);
		PrintVersions("# Used: " + usage.Used.Count, usage.Used);
		PrintVersions("# Used, but not installed: " + usage.UsedNotInstalled.Count, usage.UsedNotInstalled);
		PrintVersions("# Installed, but not used: " + usage.InstalledNotUsed.Count, usage.InstalledNotUsed);
	}
}