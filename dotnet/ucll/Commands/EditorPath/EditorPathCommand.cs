internal class EditorPathCommand(UnityHub unityHub) : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		string version = settings.GetVersionOrPrompt(unityHub);
		string path = unityHub.GetEditorPath(version);
		AnsiConsole.WriteLine(path);
		return 0;
	}
}