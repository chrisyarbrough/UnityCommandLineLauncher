using System.Diagnostics;
using Spectre.Console;

static class UnityLauncherHelper
{
	public static void LaunchUnity(string editorPath, string projectPath, string[] additionalArgs)
	{
		var args = new List<string> { "-projectPath", projectPath };
		args.AddRange(additionalArgs);

		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = Path.Combine(editorPath, "Contents/MacOS/Unity"),
				Arguments = string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)),
			},
		};

		if (!process.Start())
			throw new Exception("Failed to launch Unity");

		AnsiConsole.MarkupLine("[green]Unity Editor launched successfully[/]");
	}
}