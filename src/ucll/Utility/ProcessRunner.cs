internal interface IProcessRunner
{
	public Process Run(ProcessStartInfo startInfo);
}

internal class ProcessRunner : IProcessRunner
{
	public static readonly IProcessRunner Default = new ProcessRunner();

	public virtual Process Run(ProcessStartInfo startInfo)
	{
		string args = startInfo.Arguments;
		var timer = new ProfilingTimer($"{startInfo.FileName} {args[..Math.Min(42, args.Length)]}");

		var process = new Process
		{
			StartInfo = startInfo,
			EnableRaisingEvents = true,
		};
		process.Exited += (_, _) => timer.Stop();
		process.Start();

		return process;
	}

	public static string JoinQuoted(List<string> args)
	{
		return string.Join(" ", args.Select(a => a.Contains(' ') ? $"\"{a}\"" : a));
	}
}

internal class DryRunProcessRunner : ProcessRunner
{
	public static readonly IProcessRunner DryRun = new DryRunProcessRunner();

	public override Process Run(ProcessStartInfo startInfo)
	{
		AnsiConsole.MarkupLine($"[dim][[DryRun]] {startInfo.FileName} {startInfo.Arguments}[/]");
		startInfo.FileName = "true";
		return base.Run(startInfo);
	}
}