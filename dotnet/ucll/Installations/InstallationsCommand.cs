class InstallationsCommand : BaseCommand<InstallationsSettings>
{
	protected override int ExecuteImpl(InstallationsSettings settings)
	{
		var editorVersions = UnityHub.ListInstalledEditors().Select(i => i.Version).ToHashSet();
		var usedEditorVersions = FindUnityProjects().Select(p => ProjectVersionFile.Parse(p, out string _).Version)
			.ToHashSet();

		Console.WriteLine("Installed Unity versions: " + editorVersions.Count);
		foreach (string version in editorVersions.Order())
		{
			Console.WriteLine(version);
		}

		Console.WriteLine("Used Unity versions: " + usedEditorVersions.Count);
		foreach (string version in usedEditorVersions)
		{
			Console.WriteLine(version);
		}

		Console.WriteLine("Used versions that are not installed: ");
		foreach (string version in usedEditorVersions.Except(editorVersions).Order())
		{
			Console.WriteLine(version);
		}

		Console.WriteLine("Installed versions that are not used: ");
		foreach (string version in editorVersions.Except(usedEditorVersions).Order())
		{
			Console.WriteLine(version);
		}

		return 0;
	}

	private static IEnumerable<string> FindUnityProjects()
	{
		var startInfo = PlatformHelper.GetUnityProjectSearchProcess();
		startInfo.RedirectStandardOutput = true;
		var process = ProcessRunner.Default.Run(startInfo);
		process.WaitForExit();
		while (!process.StandardOutput.EndOfStream)
			yield return process.StandardOutput.ReadLine()!;
	}
}