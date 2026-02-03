internal static class TestUtil
{
	public static async Task WithinTempDirectoryAsync(Func<DirectoryInfo, Task> action)
	{
		string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		DirectoryInfo directory = Directory.CreateDirectory(path);

		try
		{
			await action.Invoke(directory);
		}
		finally
		{
			directory.Delete(recursive: true);
		}
	}

	public static void WithinTempDirectory(Action<DirectoryInfo> action)
	{
		string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		DirectoryInfo directory = Directory.CreateDirectory(path);

		try
		{
			action.Invoke(directory);
		}
		finally
		{
			directory.Delete(recursive: true);
		}
	}
}