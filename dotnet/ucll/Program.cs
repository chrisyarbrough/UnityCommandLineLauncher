using Microsoft.Extensions.DependencyInjection;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var services = new ServiceCollection();

services.AddSingleton(PlatformSupport.Create());
services.AddSingleton<UnityHub>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
	config.SetApplicationName("ucll");
	config.SetApplicationVersion("0.1.0");
	config.AddExample("open");
	config.AddExample("open", "searchPath", "--code-editor");
	config.AddExample("open", "searchPath", "--", "-batchmode", "-quit");

	config.AddCommand<OpenCommand>("open")
		.WithDescription("Open Unity Editor for a project search path or via recent projects prompt")
		.WithExample("open")
		.WithExample("open", "--favorite")
		.WithExample("open", ".")
		.WithExample("open", "searchPath", "--code-editor")
		.WithExample("open", "searchPath", "--", "-batchmode", "-quit");

	config.AddCommand<CreateCommand>("create")
		.WithDescription("Create a new Unity project")
		.WithExample("create", "path/to/new/project")
		.WithExample("create", "path/to/new/project", "--minimal")
		.WithExample("create", "path/to/new/project", "2023.2.10f1");

	config.AddCommand<ProjectPathCommand>("project-path")
		.WithDescription("Get Unity project root directory from search path or via recent projects prompt")
		.WithExample("project-path")
		.WithExample("project-path", "--favorite")
		.WithExample("project-path", ".")
		.WithExample("project-path", "searchPath");

	config.AddCommand<EditorRevisionCommand>("editor-revision")
		.WithDescription("Get revision for Unity version")
		.WithExample("editor-revision")
		.WithExample("editor-revision", "2022.3.10f1");

	config.AddCommand<EditorPathCommand>("editor-path")
		.WithDescription("Get installation path for Unity version")
		.WithExample("editor-path")
		.WithExample("editor-path", "2022.3.10f1");

	config.AddCommand<ProjectVersionCommand>("project-version")
		.WithDescription("Get Unity version from project search directory or ProjectVersion.txt")
		.WithExample("project-version")
		.WithExample("project-version", "--favorite")
		.WithExample("project-version", "searchPath");

	config.AddCommand<VersionUsageCommand>("version-usage")
		.WithDescription("List installed Unity Editor versions and indicate which ones are used by projects")
		.WithExample("version-usage")
		.WithExample("version-usage", "--plaintext");

	config.AddCommand<ProjectsUsingVersionCommand>("projects-using")
		.WithDescription("Find all projects that use a specific Unity version")
		.WithExample("projects-using")
		.WithExample("projects-using", "2022.3.10f1");

	config.AddCommand<InstallCommand>("install")
		.WithDescription("Install Unity Editor version")
		.WithExample("install", "2022.3.10f1")
		.WithExample("install", "2022.3.10f1", "--dry-run")
		.WithExample("install", "2022.3.10f1", "ff3792e53c62")
		.WithExample("install", "2022.3.10f1", "--", "--module", "ios");

	config.AddCommand<InstallMissingCommand>("install-missing")
		.WithDescription("Install all Unity versions that are used by projects but not currently installed")
		.WithExample("install-missing")
		.WithExample("install-missing", "--dry-run")
		.WithExample("install-missing", "--", "--module", "ios");

	config.AddCommand<UninstallUnusedCommand>("uninstall-unused")
		.WithDescription("Uninstall all Unity versions that are not used by any projects")
		.WithExample("uninstall-unused")
		.WithExample("uninstall-unused", "--dry-run");

	config.AddCommand<UpmGitUrlCommand>("upm-git-url")
		.WithDescription("Generate a git URL for Unity Package Manager from a Unity project")
		.WithExample("upm-git-url")
		.WithExample("upm-git-url", "--favorite")
		.WithExample("upm-git-url", "searchPath");

	config.AddCommand<HubCommand>("hub")
		.WithDescription("Execute Unity Hub interactively or with additional CLI arguments")
		.WithExample("hub")
		.WithExample("hub", "-- --headless help");
});

return app.Run(args);