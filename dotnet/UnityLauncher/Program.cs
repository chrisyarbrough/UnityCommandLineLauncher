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

	// Install editor
	config.AddCommand<InstallCommand>("install")
		.WithDescription("Install Unity Editor version")
		.WithExample("install", "2022.3.10f1")
		.WithExample("install", "2022.3.10f1", "abcd1234");

	// Get revision
	config.AddCommand<EditorRevisionCommand>("editor-revision")
		.WithDescription("Get revision for Unity version")
		.WithExample("revision", "2022.3.10f1");

	// Get editor path
	config.AddCommand<EditorPathCommand>("editor-path")
		.WithDescription("Get installation path for Unity version")
		.WithExample("editor-path", "2022.3.10f1");

	// Get project version
	config.AddCommand<ProjectVersionCommand>("project-version")
		.WithDescription("Extract Unity version from project (search directory or path to ProjectVersion.txt)")
		.WithExample("project-version", "/path/to/UnityProjectSearchDirectory");
});

return app.Run(args);