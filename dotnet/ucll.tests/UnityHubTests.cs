public class UnityHubTests
{
	[Theory]
	[InlineData(
		@"6000.0.58f2 installed at C:\Program Files\Unity\Hub\Editor\6000.0.58f2\Editor\Unity.exe",
		"6000.0.58f2",
		@"C:\Program Files\Unity\Hub\Editor\6000.0.58f2\Editor\Unity.exe")]
	[InlineData(
		"6000.0.59f2 (Apple silicon) installed at /Applications/Unity/Hub/Editor/6000.0.59f2/Unity.app",
		"6000.0.59f2",
		"/Applications/Unity/Hub/Editor/6000.0.59f2/Unity.app")]
	public void ParseEditorsOutput_WithSingleLine_ReturnsExpectedEditorInfo(
		string input,
		string expectedVersion,
		string expectedPath)
	{
		var result = UnityHub.ParseEditorsOutput(input);

		Assert.Single(result);
		Assert.Equal(expectedVersion, result[0].Version);
		Assert.Equal(expectedPath, result[0].Path);
	}

	[Fact]
	public void ParseEditorsOutput_WithMultipleLines_ReturnsExpectedEditorInfo()
	{
		// Also handle mixed line-ending output on Windows (can't use Environment.NewLine).
		const string hubOutput = "\r\n6000.0.58f2 installed at C:\\Program Files\\Unity\\Hub\\Editor\\6000.0.58f2\\Editor\\Unity.exe\n4000.0.59f2 installed at C:\\Program Files\\Unity\\Hub\\Editor\\4000.0.59f2\\Editor\\Unity.exe\n";
		var result = UnityHub.ParseEditorsOutput(hubOutput);
		Assert.Equal(2, result.Count);
		Assert.Equal("6000.0.58f2", result[0].Version);
		Assert.Equal(@"C:\Program Files\Unity\Hub\Editor\6000.0.58f2\Editor\Unity.exe", result[0].Path);
		Assert.Equal("4000.0.59f2", result[1].Version);
		Assert.Equal(@"C:\Program Files\Unity\Hub\Editor\4000.0.59f2\Editor\Unity.exe", result[1].Path);
	}
}