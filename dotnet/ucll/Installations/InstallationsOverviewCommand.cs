class InstallationsOverviewCommand : BaseCommand<InstallationsOverviewSettings>
{
	protected override int ExecuteImpl(InstallationsOverviewSettings settings)
	{
		var editorVersions = UnityHub.ListInstalledEditors().Select(i => i.Version).ToHashSet();
		var usedEditorVersions = FindUnityProjects().Select(p => ProjectVersionFile.Parse(p, out string _).Version)
			.ToHashSet();

		if (settings.Parseable)
			PrintParseable(editorVersions, usedEditorVersions);
		else
			PrintTable(editorVersions, usedEditorVersions);

		return 0;
	}

	private static void PrintTable(HashSet<string> editorVersions, HashSet<string> usedEditorVersions)
	{
		var table = new Table();
		table.Border(TableBorder.Rounded);
		table.AddColumn("[bold]Version[/]");
		table.AddColumn("[bold]Installed[/]");
		table.AddColumn("[bold]Used[/]");

		foreach (string version in editorVersions.Union(usedEditorVersions).Order())
		{
			bool isInstalled = editorVersions.Contains(version);
			bool isUsed = usedEditorVersions.Contains(version);

			static string GetIcon(bool value) => value ? "[green]✓[/]" : "[red]✗[/]";

			table.AddRow(Markup.Escape(version), GetIcon(isInstalled), GetIcon(isUsed));
		}

		AnsiConsole.Write(table);

		AnsiConsole.WriteLine();
		AnsiConsole.MarkupLine($"[bold]Summary:[/]");
		AnsiConsole.MarkupLine($"  Installed versions: [cyan]{editorVersions.Count}[/]");
		AnsiConsole.MarkupLine($"  Used versions: [cyan]{usedEditorVersions.Count}[/]");
		AnsiConsole.MarkupLine($"  Used but not installed: [yellow]{usedEditorVersions.Except(editorVersions).Count()}[/]");
		AnsiConsole.MarkupLine($"  Installed but not used: [yellow]{editorVersions.Except(usedEditorVersions).Count()}[/]");
	}

	private static void PrintParseable(HashSet<string> editorVersions, HashSet<string> usedEditorVersions)
	{
		Console.WriteLine("Installed Unity versions: " + editorVersions.Count);
		foreach (string version in editorVersions.Order())
		{
			Console.WriteLine(version);
		}

		Console.WriteLine("Used Unity versions: " + usedEditorVersions.Count);
		foreach (string version in usedEditorVersions.Order())
		{
			Console.WriteLine(version);
		}

		Console.WriteLine("Used versions that are not installed: ");
		foreach (string version in usedEditorVersions.Except(editorVersions).Order())
		{
			Console.WriteLine(version);
		}

		Console.WriteLine("Installed versions that are not used: ");
		foreach (string version in editorVersions.Except(usedEditorVersions).Order())
		{
			Console.WriteLine(version);
		}
	}

	private static IEnumerable<string> FindUnityProjects()
	{
		var startInfo = PlatformHelper.GetUnityProjectSearchProcess();
		startInfo.RedirectStandardOutput = true;
		var process = ProcessRunner.Default.Run(startInfo);
		process.WaitForExit();
		while (!process.StandardOutput.EndOfStream)
			yield return process.StandardOutput.ReadLine()!;
	}
}