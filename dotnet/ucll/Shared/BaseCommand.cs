abstract class BaseCommand<TSettings> : Command<TSettings>
	where TSettings : CommandSettings
{
	protected CommandContext context { get; private set; } = null!;

	public sealed override int Execute(
		CommandContext context,
		TSettings settings,
		CancellationToken cancellationToken)
	{
		try
		{
			using (new ProfilingTimer(GetType().Name))
			{
				this.context = context;
				return ExecuteImpl(settings);
			}
		}
		catch (Exception ex)
		{
			WriteError(ex.Message);
			return 1;
		}
	}

	protected abstract int ExecuteImpl(TSettings settings);

	protected static void WriteError(string message) =>
		AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(message)}[/]");

	protected static void WriteSuccess(string message) => AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
}