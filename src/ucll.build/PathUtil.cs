internal static class PathUtil
{
	public static string PrepareOutputDirectory(string name)
	{
		DirectoryInfo artifactsDirectory = new(Path.Combine(AppContext.BaseDirectory, name));

		if (!artifactsDirectory.Exists)
		{
			artifactsDirectory.Create();
		}
		else
		{
			// Clean the directory without deleting the directory itself to keep terminal sessions alive.
			foreach (FileInfo file in artifactsDirectory.EnumerateFiles())
				file.Delete();

			foreach (DirectoryInfo subDir in artifactsDirectory.EnumerateDirectories())
				subDir.Delete(recursive: true);
		}

		return artifactsDirectory.FullName;
	}
}