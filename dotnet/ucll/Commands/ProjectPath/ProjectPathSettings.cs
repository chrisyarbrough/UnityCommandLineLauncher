using System.ComponentModel;

internal class ProjectPathSettings : CommandSettings
{
	[CommandArgument(0, "[searchPath]")]
	[Description(Descriptions.SearchPath)]
	public string? SearchPath { get; init; }

	[CommandOption("-f|--favorite|--favorites")]
	[Description(Descriptions.Favorite)]
	public bool Favorite { get; init; }
}