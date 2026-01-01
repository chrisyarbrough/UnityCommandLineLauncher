using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                            DynamicallyAccessedMemberTypes.PublicMethods)]
class GetProjectVersionCommand : Command<GetProjectVersionSettings>
{
	public override int Execute(
		CommandContext context,
		GetProjectVersionSettings settings,
		CancellationToken cancellationToken)
	{
		try
		{
			var versionInfo = ProjectVersionFile.Parse(settings.ProjectVersionFilePath);
			string output = versionInfo.Version;

			if (versionInfo.Changeset != null)
				output += " " + versionInfo.Changeset;

			Console.WriteLine(output);
			return 0;
		}
		catch (Exception ex)
		{
			AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
			return 1;
		}
	}
}