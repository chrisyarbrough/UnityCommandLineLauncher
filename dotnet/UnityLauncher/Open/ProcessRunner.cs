using System.Diagnostics;

interface IProcessRunner
{
	Process Run(string fileName, bool redirectOutput = false, string? args = null);
}

class ProcessRunner : IProcessRunner
{
	public Process Run(string fileName, bool redirectOutput = false, string? args = null)
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

class DryRunProcessRunner : IProcessRunner
{
	private static readonly IProcessRunner dummyProcess = new ProcessRunner();

	public Process Run(string fileName, bool redirectOutput = false, string? args = null)
	{
		AnsiConsole.WriteLine("[DryRun] " + fileName + " " + args);
		return dummyProcess.Run("true", redirectOutput, args);
	}
}