class EditorRevisionCommand : AsyncCommand<EditorRevisionSettings>
{
	public override async Task<int> ExecuteAsync(CommandContext context,
		EditorRevisionSettings settings,
		CancellationToken cancellationToken)
	{
		try
		{
			var changeset = await UnityReleaseApi.FetchChangesetAsync(settings.Version);
			Console.WriteLine(changeset);
			return 0;
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
			return 1;
		}
	}
}