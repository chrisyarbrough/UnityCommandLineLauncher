internal class VersionUsageCommand(PlatformSupport platformSupport, UnityHub unityHub)
	: BaseCommand<VersionUsageSettings>
{
	protected override int ExecuteImpl(VersionUsageSettings settings)
	{
		var versions = new VersionUsage(platformSupport, unityHub);

		if (settings.PlainText)
			PrintParseable(versions);
		else
			PrintTable(versions);

		return 0;
	}

	private static void PrintTable(VersionUsage versions)
	{
		var table = new Table();
		table.Border(TableBorder.Rounded);
		table.AddColumn("[bold]Version[/]");
		table.AddColumn("[bold]Installed[/]");
		table.AddColumn("[bold]Used[/]");

		foreach (string version in UnityVersion.SortNewestFirst(versions.Installed.Union(versions.Used)))
		{
			bool isInstalled = versions.Installed.Contains(version);
			bool isUsed = versions.Used.Contains(version);

			static string GetIcon(bool value) => value ? "[green]✓[/]" : "[red]✗[/]";

			table.AddRow(Markup.Escape(version), GetIcon(isInstalled), GetIcon(isUsed));
		}

		AnsiConsole.Write(table);

		AnsiConsole.WriteLine();
		AnsiConsole.MarkupLine("[bold]Summary:[/]");
		AnsiConsole.MarkupLine($"  Installed versions: [cyan]{versions.Installed.Count}[/]");
		AnsiConsole.MarkupLine($"  Used versions: [cyan]{versions.Used.Count}[/]");
		AnsiConsole.MarkupLine($"  Used but not installed: [yellow]{versions.UsedNotInstalled.Count}[/]");
		AnsiConsole.MarkupLine($"  Installed but not used: [yellow]{versions.InstalledNotUsed.Count}[/]");
	}

	private static void PrintParseable(VersionUsage installs)
	{
		Console.WriteLine("# Installed versions: " + installs.Installed.Count);
		foreach (string version in UnityVersion.SortNewestFirst(installs.Installed))
		{
			Console.WriteLine(version);
		}

		Console.WriteLine("# Used versions: " + installs.Used.Count);
		foreach (string version in UnityVersion.SortNewestFirst(installs.Used))
		{
			Console.WriteLine(version);
		}

		Console.WriteLine("# Used versions that are not installed: " + installs.UsedNotInstalled.Count);
		foreach (string version in UnityVersion.SortNewestFirst(installs.UsedNotInstalled))
		{
			Console.WriteLine(version);
		}

		Console.WriteLine("# Installed versions that are not used: " + installs.InstalledNotUsed.Count);
		foreach (string version in UnityVersion.SortNewestFirst(installs.InstalledNotUsed))
		{
			Console.WriteLine(version);
		}
	}
}