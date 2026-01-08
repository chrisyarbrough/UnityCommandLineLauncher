internal readonly struct ProfilingTimer : IDisposable
{
	private readonly string? label;
	private readonly Stopwatch? watch;

	public ProfilingTimer(string label)
	{
		if (Debug.IsEnabled)
		{
			this.label = label;
			this.watch = Stopwatch.StartNew();
		}
	}

	public void Stop()
	{
		if (Debug.IsEnabled)
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