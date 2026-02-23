public static class ProjectTests
{
	[Theory]
	[InlineData("")]
	[InlineData("asdf")]
	private static void ParseThrowsWhenNotParsed(string fileContent)
	{
		RunTestOnProjectVersionFile(fileContent, filePath =>
		{
			Assert.Throws<UserException>(() => Project.Parse(filePath));
		});
	}

	[Theory]
	[InlineData("""
	            m_EditorVersion: 6000.0.59f2
	            m_EditorVersionWithRevision: 6000.0.59f2 (ef281c76c3c1)
	            """, "6000.0.59f2", "ef281c76c3c1")]
	[InlineData("m_EditorVersion: 6000.0.59f2", "6000.0.59f2", null)]
	private static void ParseReturnsExpectedVersion(
		string fileContent,
		string expectedVersion,
		string? expectedChangeset)
	{
		RunTestOnProjectVersionFile(fileContent, filePath =>
		{
			var result = Project.Parse(filePath);
			Assert.Equal(expectedVersion, result.Version);
			Assert.Equal(expectedChangeset, result.Changeset);
		});
	}

	private static void RunTestOnProjectVersionFile(string fileContent, Action<string> assertions)
	{
		const string filePath = "TestData/ProjectVersion.txt";
		Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
		File.WriteAllText(filePath, fileContent);
		try
		{
			assertions.Invoke(filePath);
		}
		finally
		{
			File.Delete(filePath);
		}
	}
}