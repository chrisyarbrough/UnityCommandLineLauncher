class GetRevisionCommand : AsyncCommand<GetRevisionSettings>
{
	public override async Task<int> ExecuteAsync(CommandContext context,
		GetRevisionSettings settings,
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