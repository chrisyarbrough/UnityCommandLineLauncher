sealed class WindowsSupport : PlatformSupport
{
	protected override string GetRelativeEditorPathToExecutableCore()
		=> @"Editor\Unity.exe";

	protected override int GetInstallationLevelsToGoUp()
		=> 2;

	protected override string GetEditorPathPattern()
		=> @"C:\Program Files\Unity\Hub\Editor\{0}\Editor\Unity.exe";

	protected override string GetHubPathPattern()
		=> @"C:\Program Files\Unity Hub\Unity Hub.exe";

	protected override string GetConfigDirectoryPath()
		=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UnityHub");

	protected override IEnumerable<string?> GetPlatformSpecificHubPaths()
	{
		// Windows doesn't have platform-specific search like mdfind
		yield break;
	}

	protected override ProcessStartInfo GetUnityProjectSearchProcessCore()
	{
		// Use Windows Search index via COM (equivalent to mdfind on macOS)
		// Queries the SystemIndex database - very fast, uses existing Windows Search index
		// Note: Only searches indexed locations (configurable in Windows Search settings)
		const string script = """
		                      $connection = New-Object -ComObject ADODB.Connection
		                      $recordset = New-Object -ComObject ADODB.Recordset
		                      try {
		                      	$connection.Open('Provider=Search.CollatorDSO;Extended Properties=''Application=Windows'';')
		                      	$query = 'SELECT System.ItemPathDisplay FROM SystemIndex WHERE System.FileName = ''ProjectVersion.txt'''
		                      	$recordset.Open($query, $connection)
		                      	while (-not $recordset.EOF) {
		                      		$path = $recordset.Fields.Item('System.ItemPathDisplay').Value
		                      		if ($path -like '*\ProjectSettings\ProjectVersion.txt') {
		                      			$path
		                      		}
		                      		$recordset.MoveNext()
		                      	}
		                      } finally {
		                      	if ($recordset.State -eq 1) { $recordset.Close() }
		                      	if ($connection.State -eq 1) { $connection.Close() }
		                      }
		                      """;

		return new ProcessStartInfo("powershell.exe", $"-NoProfile -Command \"{script}\"");
	}

	protected override ProcessStartInfo GetOpenFileProcessCore(string filePath)
	{
		// Use cmd /c start with empty window title
		return new ProcessStartInfo("cmd.exe", $"/c start \"\" \"{filePath}\"");
	}

	protected override ProcessStartInfo GetOpenFileWithApplicationProcessCore(string applicationPath, string filePath)
	{
		// On Windows, directly execute the application with the file as argument
		return new ProcessStartInfo(applicationPath, $"\"{filePath}\"");
	}

	protected override string FormatHubArgsCore(string args)
		=> $"-- {args}";

	protected override string? GetScriptingEditorPathCore()
	{
		// Read from Windows registry
		const string script = """
		                      try {
		                      	$path = Get-ItemProperty -Path 'HKCU:\Software\Unity Technologies\Unity Editor 5.x' -Name 'kScriptsDefaultApp' -ErrorAction Stop
		                      	$path.'kScriptsDefaultApp'
		                      } catch {
		                      	exit 1
		                      }
		                      """;

		var process = new ProcessRunner().Run(
			new ProcessStartInfo("powershell.exe", $"-NoProfile -Command \"{script}\"")
			{
				RedirectStandardOutput = true,
			});

		var output = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();

		return process.ExitCode == 0 ? output : null;
	}
}