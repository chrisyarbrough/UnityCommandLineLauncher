class EditorPathCommand : BaseCommand<EditorPathSettings>
{
	protected override int ExecuteImpl(EditorPathSettings settings)
	{
		var path = UnityHub.GetEditorPath(settings.Version);
		AnsiConsole.WriteLine(path);
		return 0;
	}
}