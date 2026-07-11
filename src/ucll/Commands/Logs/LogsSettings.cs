using System.ComponentModel;

internal class LogsSettings : MutatingSettings
{
	[CommandArgument(0, "[searchPath]")]
	[Description(Descriptions.SearchPath)]
	public string? SearchPath { get; init; }

	[CommandOption("-f|--favorite|--favorites")]
	[Description(Descriptions.Favorite)]
	public bool Favorite { get; init; }

	[CommandOption("-t|--type")]
	[Description("Log type to open: all, editor, upm, or player.")]
	public string Type { get; init; } = "all";

	[CommandOption("-p|--path-only")]
	[Description("Print log paths without opening them.")]
	public bool PathOnly { get; init; }

	public override ValidationResult Validate()
	{
		string[] supportedTypes = ["all", "editor", "upm", "player"];
		if (!supportedTypes.Contains(Type, StringComparer.OrdinalIgnoreCase))
			return ValidationResult.Error("Log type must be one of: all, editor, upm, player.");

		return base.Validate();
	}
}
