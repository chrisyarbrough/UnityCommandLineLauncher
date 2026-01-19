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
}