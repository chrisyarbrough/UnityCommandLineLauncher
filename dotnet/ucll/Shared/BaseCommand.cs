internal abstract class BaseCommand<TSettings> : Command<TSettings>
	where TSettings : CommandSettings
{
	protected CommandContext Context { get; private set; } = null!;

	public sealed override int Execute(
		CommandContext context,
		TSettings settings,
		CancellationToken cancellationToken)
	{
		try
		{
			using (new ProfilingTimer(GetType().Name))
			{
				this.Context = context;
				return ExecuteImpl(settings);
			}
		}
		catch (UserException e)
		{
			WriteError(e.Message);
			return 1;
		}
		catch (Exception ex)
		{
			WriteError(ex);
			return 2;
		}
	}

	protected abstract int ExecuteImpl(TSettings settings);

	protected static void WriteError(Exception exception) =>
		AnsiConsole.WriteException(exception, ExceptionFormats.ShortenPaths);

	protected static void WriteError(string message) =>
		AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(message)}[/]");

	protected static void WriteSuccess(string message) => AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
}