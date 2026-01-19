public class OpenCommandTests
{
	[Fact]
	public async Task WaitForFileAsyncFindsExistingFile()
	{
		await CreateTempProject(async tempDir =>
		{
			string testFile = Path.Combine(tempDir, "test.sln");
			await File.WriteAllTextAsync(testFile, "content");

			string result = await OpenCommand.WaitForFileAsync(tempDir, "*.sln");

			Assert.Equal(testFile, result);
		});
	}

	[Fact]
	public async Task WaitForFileAsyncWaitsForNewFile()
	{
		await CreateTempProject(async tempDir =>
		{
			string testFile = Path.Combine(tempDir, "delayed.sln");

			Task<string> waitTask = OpenCommand.WaitForFileAsync(tempDir, "*.sln");

			// Simulate Unity project generating the solution file.
			await Task.Delay(100);
			await File.WriteAllTextAsync(testFile, "content");

			string result = await waitTask.WaitAsync(TimeSpan.FromSeconds(3));

			Assert.Equal(testFile, result);
		});
	}

	private static async Task CreateTempProject(Func<string, Task> action)
	{
		string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		Directory.CreateDirectory(tempDir);

		try
		{
			await action.Invoke(tempDir);
		}
		finally
		{
			Directory.Delete(tempDir, recursive: true);
		}
	}
}