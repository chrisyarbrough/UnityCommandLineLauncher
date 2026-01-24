using Spectre.Console.Cli.Testing;
using System.Text.RegularExpressions;

public class ProgramTests
{
	[Fact]
	public void Version()
	{
		string expectedVersion = FindProjectVersion("../../ucll/ucll.csproj");

		// Test that the app outputs the correct version
		var app = new CommandAppTester();
		app.Configure(AppConfiguration.Build);
		var result = app.Run("--version");
		Assert.Equal(expectedVersion, result.Output);
	}

	private static string FindProjectVersion(string projectFile)
	{
		string projectContent = File.ReadAllText(projectFile);
		Match match = Regex.Match(input: projectContent, @"<Version>(.+)<\/Version>");

		if (!match.Success)
			throw new Exception("Version could not be found in project file.");

		return match.Groups[1].Value;
	}
}