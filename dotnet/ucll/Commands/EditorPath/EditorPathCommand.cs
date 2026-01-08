internal class EditorPathCommand(UnityHub unityHub) : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		var path = unityHub.GetEditorPath(settings.Version);
		AnsiConsole.WriteLine(path);
		return 0;
	}
}