var app = new CommandApp();

app.Configure(config =>
{
	config.SetApplicationName("ucll");
	config.SetApplicationVersion("1.0.0");

	config.AddCommand<OpenCommand>("open").ConfigureOpen();

	config.AddBranch("project", command =>
	{
		command.SetDescription("Manage Unity projects");

		command.AddCommand<OpenCommand>("open").ConfigureOpen(parentName: "project");

		command.AddCommand<ProjectVersionCommand>("version")
			.WithDescription("Get Unity version from project search directory or ProjectVersion.txt")
			.WithExample("project", "version", "path");
	});

	config.AddBranch("editor", command =>
	{
		command.SetDescription("Manage a specific Unity Editor");

		command.AddCommand<InstallCommand>("install")
			.WithDescription("Install Unity Editor version")
			.WithExample("editor", "install", "2022.3.10f1")
			.WithExample("editor", "install", "2022.3.10f1", "ff3792e53c62")
			.WithExample("editor", "install", "2022.3.10f1", "--", "--module", "android");

		command.AddCommand<EditorRevisionCommand>("revision")
			.WithDescription("Get revision for Unity version")
			.WithExample("editor", "revision", "2022.3.10f1");

		command.AddCommand<EditorPathCommand>("path")
			.WithDescription("Get installation path for Unity version")
			.WithExample("editor", "path", "2022.3.10f1");

		command.AddCommand<InstallationsUsedCommand>("used")
			.WithDescription("Find all projects using a specific Unity version")
			.WithExample("editor", "used", "2022.3.10f1");
	});

	config.AddBranch("editors", command =>
	{
		command.SetDescription("Manage all installed Unity Editors");

		command.AddCommand<InstallationsOverviewCommand>("list")
			.WithDescription("List installed Unity Editor versions")
			.WithExample("editors", "list")
			.WithExample("editors", "list", "--parseable");
	});
});

return app.Run(args);

static class ICommandConfigurationExtensions
{
	public static void ConfigureOpen(this ICommandConfigurator command, string? parentName = null)
	{
		string[] MakeExample(params string[] args) => parentName == null ? args : [parentName, ..args];

		command.WithDescription("Open Unity Editor for a project search path or via recent projects prompt")
			.WithExample(MakeExample("open"))
			.WithExample(MakeExample("open", "."))
			.WithExample(MakeExample("open", "path/to/project"))
			.WithExample(MakeExample("open", "path/to/project", "--", "-batchmode", "-quit"));
	}
}