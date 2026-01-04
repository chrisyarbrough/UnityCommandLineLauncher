static class Debug
{
	public static readonly bool IsEnabled = Environment.CommandLine.Contains("--debug");

	public static void WriteLine(string message)
	{
		if (IsEnabled)
			AnsiConsole.MarkupLine($"[dim]{message}[/]");
	}
}