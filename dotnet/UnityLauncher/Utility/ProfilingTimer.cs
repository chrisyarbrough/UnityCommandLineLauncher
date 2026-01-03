using System.Diagnostics;

readonly struct ProfilingTimer : IDisposable
{
	private static readonly bool isEnabled = Environment.CommandLine.Contains("--profile");

	private readonly string? label;
	private readonly Stopwatch? watch;

	public ProfilingTimer(string label)
	{
		if (isEnabled)
		{
			this.label = label;
			this.watch = Stopwatch.StartNew();
		}
	}

	public void Stop()
	{
		if (isEnabled)
		{
			watch!.Stop();
			AnsiConsole.WriteLine($"[Timing] {label}: {watch.ElapsedMilliseconds}ms");
		}
	}

	public void Dispose()
	{
		Stop();
	}
}