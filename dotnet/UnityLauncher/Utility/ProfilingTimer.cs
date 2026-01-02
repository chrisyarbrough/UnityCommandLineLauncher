using System.Diagnostics;

readonly struct ProfilingTimer
{
	private readonly string? label;
	private readonly Stopwatch? watch;

	public ProfilingTimer(string label)
	{
		if (IsProfilingRun)
		{
			this.label = label;
			this.watch = Stopwatch.StartNew();
		}
	}

	public void Stop()
	{
		if (IsProfilingRun)
		{
			watch!.Stop();
			AnsiConsole.WriteLine($"[Timing] {label}: {watch.ElapsedMilliseconds}ms");
		}
	}

	private static bool IsProfilingRun => Environment.CommandLine.Contains("--profile");
}