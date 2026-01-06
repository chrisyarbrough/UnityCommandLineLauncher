static class SelectionPrompt
{
	public static string Prompt(IList<string> choices, string title)
	{
		string choice = PromptImpl(choices, title);

		if (string.IsNullOrEmpty(choice))
			throw new UserException("No search path specified. Aborting.");

		return choice;
	}

	private static string PromptImpl(IList<string> choices, string title)
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
					.AddChoices(choices));
		}
	}

	private static string PromptWithFzf(IList<string> projects, string title)
	{
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "fzf",
				Arguments = $"--prompt=\"{title}\" --height=40% --reverse --bind=change:first -i",
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
			},
		};

		process.Start();

		foreach (var project in projects)
			process.StandardInput.WriteLine(project);

		process.StandardInput.Close();

		string result = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();
		return result;
	}
}