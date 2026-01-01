class EditorPathCommand : Command<EditorPathSettings>
{
	public override int Execute(
		CommandContext context,
		EditorPathSettings settings,
		CancellationToken cancellationToken)
	{
		try
		{
			var path = UnityHub.GetEditorPath(settings.Version);

			if (path == null)
			{
				AnsiConsole.MarkupLine($"[red]Unity {settings.Version} is not installed[/]");
				return 1;
			}

			Console.WriteLine(path);
			return 0;
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
			return 1;
		}
	}
}