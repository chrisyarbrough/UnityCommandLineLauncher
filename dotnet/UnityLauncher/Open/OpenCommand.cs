using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                            DynamicallyAccessedMemberTypes.PublicMethods)]
class OpenCommand : Command<OpenSettings>
{
	public override int Execute(CommandContext context, OpenSettings settings, CancellationToken cancellationToken)
	{
		try
		{
			var projectDir = Path.GetFullPath(settings.ProjectPath);

			if (!Directory.Exists(projectDir))
			{
				AnsiConsole.MarkupLine(
					$"[red]Error: Project directory '{settings.ProjectPath}' is not a valid directory[/]");
				return 1;
			}

			// Find and parse project version
			var versionFile = FindProjectVersionFile(projectDir);
			AnsiConsole.MarkupLine($"[cyan]File: {versionFile}[/]");

			var version = ProjectVersionFile.Parse(versionFile);

			var changesetInfo = !string.IsNullOrEmpty(version.Changeset)
				? $" ({version.Changeset})"
				: string.Empty;
			AnsiConsole.MarkupLine($"[cyan]Version: {version.Version}{changesetInfo}[/]");

			// Ensure Unity Editor is installed
			UnityHub.EnsureEditorInstalledAsync(version.Version, version.Changeset)
				.Wait(cancellationToken);

			// Get editor path
			var editorPath = UnityHub.GetEditorPath(version.Version);
			if (editorPath == null)
			{
				AnsiConsole.MarkupLine($"[red]Error: Unity version {version.Version} is not installed[/]");
				return 1;
			}

			AnsiConsole.MarkupLine($"[cyan]Editor: {editorPath}[/]");

			UnityLauncherHelper.LaunchUnity(editorPath, projectDir, settings.UnityArgs ?? []);

			return 0;
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
			return 1;
		}
	}

	private static string FindProjectVersionFile(string searchDir)
	{
		var foundFiles = new List<string>();

		foreach (var file in Directory.EnumerateFiles(searchDir, "ProjectVersion.txt", SearchOption.AllDirectories))
		{
			// Ensure it's in a ProjectSettings directory
			if (Path.GetFileName(Path.GetDirectoryName(file)) == "ProjectSettings")
			{
				foundFiles.Add(Path.GetFullPath(file));
			}
		}

		if (foundFiles.Count == 0)
			throw new Exception($"No ProjectSettings/ProjectVersion.txt found in {searchDir}");

		if (foundFiles.Count > 1)
		{
			throw new Exception(
				$"Found multiple ProjectVersion.txt files:\n{string.Join("\n", foundFiles)}\n" +
				"Please run in a directory with only one Unity project");
		}

		return foundFiles[0];
	}
}