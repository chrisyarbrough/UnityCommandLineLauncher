class ProjectVersionCommand : Command<ProjectVersionSettings>
{
	public override int Execute(
		CommandContext context,
		ProjectVersionSettings settings,
		CancellationToken cancellationToken)
	{
		try
		{
			var versionInfo = ProjectVersionFile.Parse(settings.SearchDirectory);
			string output = versionInfo.Version;

			if (versionInfo.Changeset != null)
				output += " " + versionInfo.Changeset;

			Console.WriteLine(output);
			return 0;
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
			return 1;
		}
	}
}