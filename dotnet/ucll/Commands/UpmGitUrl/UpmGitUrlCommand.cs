internal class UpmGitUrlCommand(UnityHub unityHub) : SearchPathCommand<UpmGitUrlSettings>(unityHub)
{
	protected override int ExecuteImpl(UpmGitUrlSettings settings)
	{
		string searchPath = ResolveSearchPath(settings.SearchPath, settings.Favorite);

		string projectPath = Project.FindProjectPath(searchPath);

		// Find package.json files in the Packages directory
		string packagesDir = Path.Combine(projectPath, "Packages");
		if (!Directory.Exists(packagesDir))
			throw new UserException($"Packages directory not found in the project: {projectPath}");

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
				.Select(p => Path.GetRelativePath(projectPath, p))
				.ToArray();
			string relativeChoice = SelectionPrompt.Prompt(choices, "Multiple package.json files found. Select one: ");
			selectedPackageJson = Path.Combine(projectPath, relativeChoice);
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

		(string output, string error, int exitCode) = process.CaptureOutput();

		if (exitCode != 0)
			throw new UserException($"Git command failed: {error}");

		return output.Trim();
	}
}