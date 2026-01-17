using System.Diagnostics.CodeAnalysis;

internal static class SelectionPrompt
{
	public static string Prompt(IEnumerable<string> choices, string title)
	{
		string choice = PromptImpl(choices, title);

		if (string.IsNullOrEmpty(choice))
			throw new UserCancelledException("Selection prompt cancelled.");

		return choice;
	}

	[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
	private static string PromptImpl(IEnumerable<string> choices, string title)
	{
		try
		{
			// This supports fuzzy matching and acronyms.
			return PromptWithFzf(choices, title);
		}
		catch (Exception)
		{
			return AnsiConsole.Prompt(
				new SelectionPrompt<string>()
					.Title(title)
					.EnableSearch()
					.SearchPlaceholderText("[dim](Type to search. Install 'fzf' for fuzzy matching.)[/]")
					.WrapAround()
					.AddChoices(choices));
		}
	}

	private static string PromptWithFzf(IEnumerable<string> projects, string title)
	{
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "fzf",
				Arguments = $"--prompt=\"{title}\" --height=40% --reverse --bind=change:first --cycle",
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
			},
		};

		process.Start();

		foreach (string project in projects)
			process.StandardInput.WriteLine(project);

		process.StandardInput.Close();

		return process.CaptureOutput().output.Trim();
	}
}