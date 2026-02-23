public class TestBase
{
	protected static async Task WithinTempDirectoryAsync(Func<DirectoryInfo, Task> action)
	{
		DirectoryInfo directory = CreateTempDirectory();

		try
		{
			await action.Invoke(directory);
		}
		finally
		{
			directory.Delete(recursive: true);
		}
	}

	protected static void WithinTempDirectory(Action<DirectoryInfo> action)
	{
		WithinTempDirectoryAsync(t =>
		{
			action.Invoke(t);
			return Task.CompletedTask;
		}).Wait();
	}

	private static DirectoryInfo CreateTempDirectory()
	{
		string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		return Directory.CreateDirectory(path);
	}
}