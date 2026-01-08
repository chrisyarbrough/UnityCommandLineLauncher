public static class ProjectVersionFileTests
{
	[Theory]
	[InlineData("""
	            m_EditorVersion: 6000.0.59f2
	            m_EditorVersionWithRevision: 6000.0.59f2 (ef281c76c3c1)
	            """, "6000.0.59f2", "ef281c76c3c1")]
	[InlineData("m_EditorVersion: 6000.0.59f2", "6000.0.59f2", null)]
	private static void ParseReturnsExpectedVersion(string fileContent,
		string expectedVersion,
		string? expectedChangeset)
	{
		const string filePath = "TestData/ProjectVersion.txt";
		Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
		File.WriteAllText(filePath, fileContent);
		try
		{
			var result = ProjectVersionFile.Parse(filePath, out string _);

			Assert.Equal(expectedVersion, result.Version);
			Assert.Equal(expectedChangeset, result.Changeset);
		}
		finally
		{
			File.Delete(filePath);
		}
	}
}