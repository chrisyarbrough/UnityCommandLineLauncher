internal class UpmGitUrlCommand(UnityHub unityHub) : BaseCommand<UpmGitUrlSettings>
{
	protected override int ExecuteImpl(UpmGitUrlSettings settings)
	{
		string searchPath = settings.SearchPath ?? OpenCommand.PromptForRecentProject(settings.Favorite, unityHub);
		searchPath = Path.GetFullPath(searchPath);

		if (!Directory.Exists(searchPath) && !File.Exists(searchPath))
		{
			WriteError($"'{searchPath}' does not exist.");
			return 1;
		}

		string projectVersionPath = ProjectVersionFile.FindFilePath(searchPath);
		string projectDir = new FileInfo(projectVersionPath).Directory!.Parent!.FullName;

		// Find package.json files in the Packages directory
		string packagesDir = Path.Combine(projectDir, "Packages");
		if (!Directory.Exists(packagesDir))
			throw new UserException($"Packages directory not found in the project: {projectDir}");

		string[] packageJsonFiles = Directory.GetFiles(packagesDir, "package.json", SearchOption.AllDirectories);

		if (packageJsonFiles.Length == 0)
			throw new UserException($"No package.json files found in {packagesDir}");

		string selectedPackageJson;
		if (packageJsonFiles.Length == 1)
		{
			selectedPackageJson = packageJsonFiles[0];
		}
		else
		{
			string[] choices = packageJsonFiles
				.Select(p => Path.GetRelativePath(projectDir, p))
				.ToArray();
			string relativeChoice = SelectionPrompt.Prompt(choices, "Multiple package.json files found. Select one: ");
			selectedPackageJson = Path.Combine(projectDir, relativeChoice);
		}

		string packageDir = new FileInfo(selectedPackageJson).Directory!.FullName;

		// This can either be the main repo root or a submodule root.
		string gitRoot = RunGitCommand(packageDir, "rev-parse --show-toplevel");

		string relativePath = Path.GetRelativePath(gitRoot, packageDir);

		// Normalize path separators to forward slashes for git URL
		relativePath = relativePath.Replace(Path.DirectorySeparatorChar, '/');

		string remoteUrl = RunGitCommand(gitRoot, "config --get remote.origin.url");
		string branch = RunGitCommand(gitRoot, "rev-parse --abbrev-ref HEAD");

		string upmUrl = $"{remoteUrl}?path={relativePath}#{branch}";
		AnsiConsole.WriteLine(upmUrl);
		return 0;
	}

	private static string RunGitCommand(string workingDirectory, string arguments)
	{
		var processInfo = new ProcessStartInfo
		{
			FileName = "git",
			Arguments = arguments,
			WorkingDirectory = workingDirectory,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		};

		using var process = ProcessRunner.Default.Run(processInfo) ??
		                    throw new UserException("Failed to start git process");

		string output = process.StandardOutput.ReadToEnd();
		string error = process.StandardError.ReadToEnd();
		process.WaitForExit();

		if (process.ExitCode != 0)
			throw new UserException($"Git command failed: {error}");

		return output.Trim();
	}
}