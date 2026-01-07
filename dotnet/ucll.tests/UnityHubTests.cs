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
	public void ParseEditorsOutput_WithSingleLine_ReturnsExpectedEditorInfo(string input, string expectedVersion, string expectedPath)
	{
		var result = UnityHub.ParseEditorsOutput(input);

		Assert.Single(result);
		Assert.Equal(expectedVersion, result[0].Version);
		Assert.Equal(expectedPath, result[0].Path);
	}
}
