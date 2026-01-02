abstract class BaseCommand<TSettings> : Command<TSettings>
	where TSettings : CommandSettings
{
	public sealed override int Execute(
		CommandContext context,
		TSettings settings,
		CancellationToken cancellationToken)
	{
		var timer = new ProfilingTimer(GetType().Name);
		try
		{
			return ExecuteImpl(settings);
		}
		catch (Exception ex)
		{
			WriteError(ex.Message);
			return 1;
		}
		finally
		{
			timer.Stop();
		}
	}

	protected abstract int ExecuteImpl(TSettings settings);

	protected static void WriteError(string message) => AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(message)}[/]");
	protected static void WriteSuccess(string message) => AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
}