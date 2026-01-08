internal class EditorRevisionCommand : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		string changeset = UnityReleaseApi.FetchChangesetAsync(settings.Version).Result;
		AnsiConsole.WriteLine(changeset);
		return 0;
	}
}