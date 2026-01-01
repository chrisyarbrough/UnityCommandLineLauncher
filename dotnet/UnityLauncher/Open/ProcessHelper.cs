using System.Diagnostics;

static class ProcessHelper
{
	public static Process Run(string fileName, bool redirectOutput = false, params IList<string> args)
	{
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = fileName,
				Arguments = string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)),
				RedirectStandardOutput = redirectOutput,
				RedirectStandardError = redirectOutput,
			},
		};

		if (!process.Start())
			throw new Exception("Failed to launch Unity");

		return process;
	}
}