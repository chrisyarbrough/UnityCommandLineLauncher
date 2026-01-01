class InstallCommand : AsyncCommand<InstallSettings>
{
	public override async Task<int> ExecuteAsync(CommandContext context,
		InstallSettings settings,
		CancellationToken cancellationToken)
	{
		try
		{
			await UnityHub.EnsureEditorInstalledAsync(settings.Version, settings.Changeset);
			AnsiConsole.MarkupLine($"[green]Unity {settings.Version} is installed and ready to use[/]");
			return 0;
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
			return 1;
		}
	}
}