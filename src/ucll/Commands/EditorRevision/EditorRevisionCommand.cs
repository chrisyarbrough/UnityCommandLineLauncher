internal class EditorRevisionCommand(UnityHub unityHub) : BaseCommand<VersionSettings>
{
	protected override int ExecuteImpl(VersionSettings settings)
	{
		string version = settings.GetVersionOrPrompt(unityHub);
		string changeset = UnityReleaseApi.FetchChangesetAsync(version).Result;
		Console.WriteLine(changeset);
		return 0;
	}
}