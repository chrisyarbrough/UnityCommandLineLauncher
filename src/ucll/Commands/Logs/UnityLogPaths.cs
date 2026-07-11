using System.Text.RegularExpressions;

internal static partial class UnityLogPaths
{
	public static IReadOnlyList<UnityLogPath> Resolve(
		string projectPath,
		string projectArgs,
		PlatformSupport platformSupport)
	{
		(string companyName, string productName) = ReadPlayerLogNames(projectPath);
		string editorLogPath = GetCustomEditorLogPath(projectArgs) ?? platformSupport.DefaultEditorLogPath;

		return
		[
			new("editor", editorLogPath),
			new("upm", platformSupport.DefaultUpmLogPath),
			new("player", platformSupport.GetPlayerLogPath(companyName, productName)),
		];
	}

	internal static string? GetCustomEditorLogPath(string projectArgs)
	{
		string[] args = SplitCommandLine(projectArgs);

		for (int i = 0; i < args.Length - 1; i++)
		{
			if (args[i].Equals("-logFile", StringComparison.OrdinalIgnoreCase)
				&& args[i + 1] != "-")
			{
				return args[i + 1];
			}
		}

		return null;
	}

	internal static (string CompanyName, string ProductName) ReadPlayerLogNames(string projectPath)
	{
		string projectSettingsPath = Path.Combine(projectPath, "ProjectSettings", "ProjectSettings.asset");
		if (!File.Exists(projectSettingsPath))
			return ("CompanyName", "ProductName");

		string companyName = "CompanyName";
		string productName = "ProductName";

		foreach (string line in File.ReadLines(projectSettingsPath))
		{
			Match companyMatch = CompanyNameRegex().Match(line);
			if (companyMatch.Success)
				companyName = companyMatch.Groups[1].Value.Trim();

			Match productMatch = ProductNameRegex().Match(line);
			if (productMatch.Success)
				productName = productMatch.Groups[1].Value.Trim();
		}

		return (companyName, productName);
	}

	private static string[] SplitCommandLine(string commandLine)
	{
		var args = new List<string>();
		MatchCollection matches = CommandLineArgRegex().Matches(commandLine);

		foreach (Match match in matches)
		{
			string value = match.Groups["quoted"].Success
				? match.Groups["quoted"].Value
				: match.Groups["bare"].Value;

			if (!string.IsNullOrWhiteSpace(value))
				args.Add(value);
		}

		return args.ToArray();
	}

	[GeneratedRegex(@"^\s*companyName:\s*(.+)\s*$")]
	private static partial Regex CompanyNameRegex();

	[GeneratedRegex(@"^\s*productName:\s*(.+)\s*$")]
	private static partial Regex ProductNameRegex();

	[GeneratedRegex("""(?:"(?<quoted>[^"]*)"|(?<bare>\S+))""")]
	private static partial Regex CommandLineArgRegex();
}

internal record UnityLogPath(string Type, string Path);
