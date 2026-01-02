abstract class BaseCommand<TSettings> : Command<TSettings>
	where TSettings : CommandSettings
{
	public sealed override int Execute(
		CommandContext context,
		TSettings settings,
		CancellationToken cancellationToken)
	{
		try
		{
			return ExecuteImpl(settings);
		}
		catch (Exception ex)
		{
			WriteError(ex.Message);
			return 1;
		}
	}

	protected abstract int ExecuteImpl(TSettings settings);

	protected static void WriteError(string message) => AnsiConsole.MarkupLine($"[red]Error: {message}[/]");
	protected static void WriteSuccess(string message) => AnsiConsole.MarkupLine($"[green]{message}[/]");
}