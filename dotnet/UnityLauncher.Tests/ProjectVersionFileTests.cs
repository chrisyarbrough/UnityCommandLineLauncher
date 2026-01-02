public static class ProjectVersionFileTests
{
	[Theory]
	[InlineData("ProjectVersionWithRevision.txt", "6000.0.59f2", "ef281c76c3c1")]
	[InlineData("ProjectVersionWithoutRevision.txt", "6000.0.59f2", null)]
	private static void ParseReturnsExpectedVersion(string filePath, string expectedVersion, string? expectedChangeset)
	{
		var testFilePath = Path.Combine("TestData", filePath);

		var result = ProjectVersionFile.Parse(testFilePath, out string _);

		Assert.Equal(expectedVersion, result.Version);
		Assert.Equal(expectedChangeset, result.Changeset);
	}
}