internal class EditorRevisionCommand : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		var changeset = UnityReleaseApi.FetchChangesetAsync(settings.Version).Result;
		AnsiConsole.WriteLine(changeset);
		return 0;
	}
}