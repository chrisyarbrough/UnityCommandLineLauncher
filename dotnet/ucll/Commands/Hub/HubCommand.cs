internal class HubCommand(UnityHub unityHub) : BaseCommand<HubCommand.HubSettings>
{
	internal class HubSettings : CommandSettings;

	protected override int ExecuteImpl(HubSettings settings)
	{
		string args = string.Join(" ", context.Remaining.Raw);

		var startInfo = new ProcessStartInfo(unityHub.GetHubPath(), args);

		bool isHeadless = args.Contains("--headless");

		// Silence output when launching interactively.
		bool redirect = !isHeadless;
		startInfo.RedirectStandardOutput = redirect;
		startInfo.RedirectStandardError = redirect;

		var process = ProcessRunner.Default.Run(startInfo);

		// Block for regular CLI commands.
		if (isHeadless)
		{
			process.WaitForExit();
			return process.ExitCode;
		}

		// Launched interactively and detached.
		return 0;
	}
}