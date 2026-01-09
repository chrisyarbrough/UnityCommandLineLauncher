public class UnityReleaseApiTests
{
	[Fact]
	public async Task FetchChangesetAsync_WithRealApi_ReturnsChangesetForKnownVersion()
	{
		string result = await UnityReleaseApi.FetchChangesetAsync("6000.3.2f1");
		Assert.Equal("a9779f353c9b", result);
	}

	[Fact]
	public async Task FetchChangesetAsync_WithInvalidVersion_ThrowsException()
	{
		await Assert.ThrowsAsync<UserException>(() => UnityReleaseApi.FetchChangesetAsync("invalid.version.xyz"));
	}
}