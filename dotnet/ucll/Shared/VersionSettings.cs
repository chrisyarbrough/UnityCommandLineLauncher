using System.ComponentModel;

internal class VersionSettings : CommandSettings
{
	[CommandArgument(0, "[version]")]
	[Description(Descriptions.Version)]
	public required string? Version { get; init; }

	public string GetVersionOrPrompt(UnityHub unityHub)
	{
		if (Version != null)
			return Version;

		return PromptForVersion(unityHub);
	}

	public static string PromptForVersion(UnityHub unityHub)
	{
		AnsiConsole.MarkupLine("[dim]No version specified. Searching for available editors...[/]");
		var versions = unityHub.ListInstalledEditors().Select(editor => editor.Version);
		return SelectionPrompt.Prompt(UnityVersion.SortNewestFirst(versions), "Select Unity version");
	}
}