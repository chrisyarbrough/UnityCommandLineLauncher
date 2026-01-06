class EditorPathCommand : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		var path = UnityHub.GetEditorPath(settings.Version);
		AnsiConsole.WriteLine(path);
		return 0;
	}
}