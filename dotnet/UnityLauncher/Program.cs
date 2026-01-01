using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
	config.SetApplicationVersion("1.0.0");

	// Open command
	config.AddCommand<OpenCommand>("open")
		.WithDescription("Open Unity Editor for a project")
		.WithExample("open", ".")
		.WithExample("open", "/path/to/project")
		.WithExample("open", "/path/to/project", "-batchmode", "-quit");

	// Get revision subcommand
	config.AddCommand<GetRevisionCommand>("get-revision")
		.WithDescription("Get the revision/changeset for a Unity version")
		.WithExample("get-revision", "2022.3.10f1");

	// Get project version subcommand
	config.AddCommand<GetProjectVersionCommand>("get-project-version")
		.WithDescription("Extract Unity version from ProjectVersion.txt file")
		.WithExample("get-project-version", "/path/to/ProjectSettings/ProjectVersion.txt");
});

return app.Run(args);