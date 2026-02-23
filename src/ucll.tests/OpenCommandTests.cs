using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli.Testing;
using Xunit.Abstractions;

public class OpenCommandTests(ITestOutputHelper output) : TestBase
{
	[Fact]
	public async Task WaitForFileAsyncFindsExistingFile()
	{
		await WithinTempDirectoryAsync(async tempDir =>
		{
			string testFile = Path.Combine(tempDir.FullName, "test.sln");
			await File.WriteAllTextAsync(testFile, "content");

			string result = await OpenCommand.WaitForFileAsync(tempDir.FullName, "*.sln");

			Assert.Equal(testFile, result);
		});
	}

	[Fact]
	public async Task WaitForFileAsyncWaitsForNewFile()
	{
		await WithinTempDirectoryAsync(async tempDir =>
		{
			string testFile = Path.Combine(tempDir.FullName, "delayed.sln");

			Task<string> waitTask = OpenCommand.WaitForFileAsync(tempDir.FullName, "*.sln");

			// Simulate Unity project generating the solution file.
			await Task.Delay(100);
			await File.WriteAllTextAsync(testFile, "content");

			string result = await waitTask.WaitAsync(TimeSpan.FromSeconds(3));

			Assert.Equal(testFile, result);
		});
	}

	[Fact]
	public void OpenPath_ProjectRoot()
	{
		RunOpenCommandTest(projectPath => projectPath);
	}

	[Fact]
	public void OpenPath_WithinProject()
	{
		RunOpenCommandTest(projectPath => Path.Combine(projectPath, "Assets"));
	}

	[Fact]
	public void OpenPath_ParentOfProject()
	{
		RunOpenCommandTest(projectPath => new DirectoryInfo(projectPath).Parent!.FullName);
	}

	private void RunOpenCommandTest(Func<string, string> modifyPathArgument)
	{
		var services = new ServiceCollection();
		services.AddSingleton(PlatformSupport.Create());
		services.AddSingleton<UnityHub>();

		var registrar = new TypeRegistrar(services);
		var app = new CommandAppTester(registrar);
		app.Configure(AppConfiguration.Build);

		WithinTempDirectory(tempDir =>
		{
			string projectPath = Path.Combine(tempDir.FullName, "Container", "TestProject");
			output.WriteLine("Creating project at: " + projectPath);
			var createResult = app.Run("create", projectPath, "6000.0.64f1", "--minimal");
			output.WriteLine(createResult.Output);
			Assert.Equal(0, createResult.ExitCode);

			var openResult = app.Run("open", modifyPathArgument(projectPath), "--dry-run");
			output.WriteLine(openResult.Output);
			Assert.Equal(0, openResult.ExitCode);
		});
	}
}