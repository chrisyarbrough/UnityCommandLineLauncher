using System.Diagnostics;

static class ProcessHelper
{
	public static Process Run(string fileName, bool redirectOutput = false, string? args = null)
	{
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = fileName,
				Arguments = args,
				RedirectStandardOutput = redirectOutput,
				RedirectStandardError = redirectOutput,
			},
		};

		if (!process.Start())
			throw new Exception("Failed to launch Unity");

		return process;
	}
}