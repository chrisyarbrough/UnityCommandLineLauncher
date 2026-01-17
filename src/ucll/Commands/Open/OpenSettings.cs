using System.ComponentModel;

internal class OpenSettings : MutatingSettings
{
	[CommandArgument(0, "[searchPath]")]
	[Description(Descriptions.SearchPath)]
	public string? SearchPath { get; init; }

	[CommandOption("-f|--favorite|--favorites")]
	[Description(Descriptions.Favorite)]
	public bool Favorite { get; init; }

	[CommandOption("-c|--code-editor")]
	[Description("Open the solution file in the default code editor")]
	public bool CodeEditor { get; init; }
}