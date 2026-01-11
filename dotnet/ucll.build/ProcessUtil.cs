using System.Diagnostics;

internal static class ProcessUtil
{
	public static Process Run(string fileName, string arguments, string? workingDirectory = null)
	{
		Process process = Process.Start(new ProcessStartInfo
		{
			FileName = fileName,
			Arguments = arguments,
			WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory(),
		})!;
		process.WaitForExit();
		return process;
	}
}