using System.ComponentModel;

class EditorPathSettings : CommandSettings
{
	[CommandArgument(0, "<version>")]
	[Description("Unity version to get the path for (e.g., 2022.3.10f1)")]
	public string Version { get; init; } = string.Empty;
}