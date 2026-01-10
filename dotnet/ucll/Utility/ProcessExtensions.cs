internal static class ProcessExtensions
{
	/// <summary>                                                                                                                                                                                                                             │
	/// Captures standard output and error streams, then waits for process completion.                                                                                                                                                        │
	/// </summary>                                                                                                                                                                                                                            │
	public static (string output, string error, int exitCode) CaptureOutput(this Process process)
	{
		static string GetStream(bool flag, StreamReader reader)
			=> flag ? reader.ReadToEnd() : string.Empty;

		ProcessStartInfo info = process.StartInfo;
		string output = GetStream(info.RedirectStandardOutput, process.StandardOutput);
		string error = GetStream(info.RedirectStandardError, process.StandardError);
		process.WaitForExit();
		return (output, error, process.ExitCode);
	}
}