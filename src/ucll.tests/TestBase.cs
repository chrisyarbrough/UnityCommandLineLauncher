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
		DirectoryInfo directory = CreateTempDirectory();

		try
		{
			action.Invoke(directory);
		}
		finally
		{
			directory.Delete(recursive: true);
		}
	}

	private static DirectoryInfo CreateTempDirectory()
	{
		string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		return Directory.CreateDirectory(path);
	}
}