public class UnityLogPathsTests
{
	[Theory]
	[InlineData("-batchmode -logFile /tmp/custom-editor.log", "/tmp/custom-editor.log")]
	[InlineData("-batchmode -logFile \"C:\\Logs\\Unity Editor.log\"", "C:\\Logs\\Unity Editor.log")]
	[InlineData("-batchmode -logfile ./Editor.log", "./Editor.log")]
	public void GetCustomEditorLogPathParsesLogFileArgument(string args, string expectedPath)
	{
		string? path = UnityLogPaths.GetCustomEditorLogPath(args);

		Assert.Equal(expectedPath, path);
	}

	[Fact]
	public void GetCustomEditorLogPathIgnoresStdout()
	{
		string? path = UnityLogPaths.GetCustomEditorLogPath("-batchmode -logFile -");

		Assert.Null(path);
	}

	[Fact]
	public void ReadPlayerLogNamesUsesProjectSettings()
	{
		CreateTempProject("""
		                  %YAML 1.1
		                  PlayerSettings:
		                    companyName: Example Studio
		                    productName: Space Tool
		                  """, tempDir =>
		{
			var names = UnityLogPaths.ReadPlayerLogNames(tempDir);

			Assert.Equal("Example Studio", names.CompanyName);
			Assert.Equal("Space Tool", names.ProductName);
		});
	}

	private static void CreateTempProject(string projectSettings, Action<string> action)
	{
		string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		string projectSettingsDir = Path.Combine(tempDir, "ProjectSettings");
		Directory.CreateDirectory(projectSettingsDir);
		File.WriteAllText(Path.Combine(projectSettingsDir, "ProjectVersion.txt"), "m_EditorVersion: 6000.0.0f1");
		File.WriteAllText(Path.Combine(projectSettingsDir, "ProjectSettings.asset"), projectSettings);

		try
		{
			action.Invoke(tempDir);
		}
		finally
		{
			Directory.Delete(tempDir, recursive: true);
		}
	}
}
