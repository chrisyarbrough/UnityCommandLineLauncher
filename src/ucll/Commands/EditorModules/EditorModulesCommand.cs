internal class EditorModulesCommand(UnityHub unityHub, PlatformSupport platformSupport) : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		VersionUsage usage = new(platformSupport, unityHub);
		string version = settings.GetVersionOrPrompt(unityHub);

		string[] modules = usage.GetInstalledModules(version);

		if (modules.Length == 0)
		{
			WriteError($"No modules found for {version}");
			return 1;
		}
		else
		{
			foreach (string moduleName in modules)
			{
				Console.WriteLine(moduleName);
			}
			return 0;
		}
	}
}