static class ConsoleHelper
{
	public static void WriteError(string message) => AnsiConsole.MarkupLine($"[red]Error: {message}[/]");
	public static void WriteSuccess(string message) => AnsiConsole.MarkupLine($"[green]{message}[/]");
}