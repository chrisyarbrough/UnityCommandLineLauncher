public class UnityReleaseApiTests
{
	[Fact]
	public async Task FetchChangesetAsync_WithRealApi_ReturnsChangesetForKnownVersion()
	{
		var result = await UnityReleaseApi.FetchChangesetAsync("6000.3.2f1");
		Assert.Equal("a9779f353c9b", result);
	}

	[Fact]
	public async Task FetchChangesetAsync_WithInvalidVersion_ThrowsException()
	{
		await Assert.ThrowsAsync<Exception>(() => UnityReleaseApi.FetchChangesetAsync("invalid.version.xyz"));
	}
}