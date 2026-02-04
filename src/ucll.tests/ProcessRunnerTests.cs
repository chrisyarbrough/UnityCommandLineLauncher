using System.Diagnostics;

public class ProcessRunnerTests
{
	[Theory]
	[InlineData(new string[] { }, "")]
	[InlineData(new[] { "single" }, "single")]
	[InlineData(new[] { "single with space" }, "\"single with space\"")]
	[InlineData(new[] { "arg1", "arg2", "arg3" }, "arg1 arg2 arg3")]
	[InlineData(new[] { "path with spaces", "normal" }, "\"path with spaces\" normal")]
	[InlineData(
		new[] { @"C:\Program Files\Unity", "arg", "another path", "simple" },
		"""
		"C:\Program Files\Unity" arg "another path" simple
		""")]
	public void JoinQuotedFormatsArgsCorrectly(string[] args, string expected)
	{
		string result = ProcessRunner.JoinQuoted([..args]);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void DryRunProcessorCanRunNoOpProcess()
	{
		// The executable name should not matter and the dry runner should use a platform-specific no-op process.
		var platformSupport = PlatformSupport.Create();
		var processor = platformSupport.CreateProcessRunner(dryRun: true);
		Process process = processor.Run(new ProcessStartInfo("NonExistingProcess"));
		process.WaitForExit();
		Assert.Equal(0, process.ExitCode);
	}
}