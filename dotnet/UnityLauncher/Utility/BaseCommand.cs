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
			ConsoleHelper.WriteError(ex.Message);
			return 1;
		}
	}

	protected abstract int ExecuteImpl(TSettings settings);
}