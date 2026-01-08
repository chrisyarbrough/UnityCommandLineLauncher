internal sealed class WindowsSupport : PlatformSupport
{
	public override ProcessStartInfo OpenFile(string filePath)
	{
		// Use cmd /c start with empty window title
		return new ProcessStartInfo("cmd.exe", $"/c start \"\" \"{filePath}\"");
	}

	public override ProcessStartInfo OpenFileWithApp(string filePath, string applicationPath)
	{
		// On Windows, directly execute the application with the file as argument
		return new ProcessStartInfo(applicationPath, $"\"{filePath}\"");
	}

	public override string RelativeEditorPathToExecutable => @"Editor\Unity.exe";

	public override string UnityHubConfigDirectory =>
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UnityHub");

	public override ProcessStartInfo GetUnityProjectSearchProcess()
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

	protected override string DefaultEditorPathTemplate => @"C:\Program Files\Unity\Hub\Editor\{0}\Editor\Unity.exe";

	protected override string DefaultUnityHubPath => @"C:\Program Files\Unity Hub\Unity Hub.exe";

	public override string? GetUnityScriptingEditorPath()
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

		string output = process.StandardOutput.ReadToEnd().Trim();
		process.WaitForExit();

		return process.ExitCode == 0 ? output : null;
	}
}