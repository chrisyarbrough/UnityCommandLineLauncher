using System.Text.Json;

internal class EditorModulesCommand(UnityHub unityHub, PlatformSupport platformSupport) : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		string version = settings.GetVersionOrPrompt(unityHub);
		string editorPath = unityHub.GetEditorPath(version);
		string installationRoot = platformSupport.FindInstallationRoot(editorPath);

		string modulesJsonPath = Path.Combine(installationRoot, "modules.json");

		if (!File.Exists(modulesJsonPath))
		{
			WriteError($"modules.json not found at {modulesJsonPath}");
			return 1;
		}

		try
		{
			string json = File.ReadAllText(modulesJsonPath);
			using JsonDocument doc = JsonDocument.Parse(json);

			var installedModules = doc.RootElement
				.EnumerateArray()
				.Where(module => module.TryGetProperty("selected", out JsonElement selected) && selected.GetBoolean())
				.Where(module => module.TryGetProperty("visible", out JsonElement visible) && visible.GetBoolean())
				.Where(module => module.TryGetProperty("name", out _))
				.Select(name => name.GetProperty("name").GetString())
				.Cast<string>();

			foreach (string moduleName in installedModules)
			{
				Console.WriteLine(moduleName);
			}

			return 0;
		}
		catch (JsonException ex)
		{
			WriteError($"Failed to parse modules.json: {ex.Message}");
			return 1;
		}
	}
}