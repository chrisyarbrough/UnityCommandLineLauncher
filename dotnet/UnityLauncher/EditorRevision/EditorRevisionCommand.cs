class EditorRevisionCommand : BaseCommand<EditorRevisionSettings>
{
	protected override int ExecuteImpl(EditorRevisionSettings settings)
	{
		var changeset = UnityReleaseApi.FetchChangesetAsync(settings.Version).Result;
		AnsiConsole.WriteLine(changeset);
		return 0;
	}
}