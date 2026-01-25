using System.ComponentModel;

internal class OpenSettings : MutatingSettings
{
	[CommandArgument(0, "[searchPath]")]
	[Description(Descriptions.SearchPath)]
	public string? SearchPath { get; init; }

	[CommandOption("-f|--favorite|--favorites")]
	[Description(Descriptions.Favorite)]
	public bool Favorite { get; init; }

	[CommandOption("--no-hub-args")]
	[Description("Do not use CLI arguments from Unity Hub")]
	public bool NoHubArgs { get; init; }

	[CommandOption("-c|--code-editor")]
	[Description("Open the solution file in the default code editor")]
	public bool CodeEditor { get; init; }

	[CommandOption("-o|--only-code-editor")]
	[Description("Open only the code editor without launching Unity")]
	public bool OnlyCodeEditor { get; init; }


	public override ValidationResult Validate()
	{
		if (CodeEditor && OnlyCodeEditor)
		{
			return ValidationResult.Error(
				"Cannot use both --code-editor and --only-code-editor. " +
				"Use --code-editor to open both Unity and the code editor, " +
				"or --only-code-editor to open only the code editor.");
		}

		return base.Validate();
	}
}