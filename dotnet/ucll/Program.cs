using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

var services = new ServiceCollection();

services.AddSingleton<PlatformSupport>(_ =>
{
	if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		return new MacSupport();

	if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		return new WindowsSupport();

	if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		return new LinuxSupport();

	throw new UserException($"Unsupported platform: {RuntimeInformation.RuntimeIdentifier}");
});

services.AddSingleton<UnityHub>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
	config.SetApplicationName("ucll");
	config.SetApplicationVersion("0.1.0");

	config.AddCommand<OpenCommand>("open")
		.WithDescription("Open Unity Editor for a project search path or via recent projects prompt")
		.WithExample("open")
		.WithExample("open", ".")
		.WithExample("open", "path/to/project")
		.WithExample("open", "path/to/project", "--", "-batchmode", "-quit");

	config.AddCommand<CreateCommand>("create")
		.WithDescription("Create a new Unity project")
		.WithExample("create", "~/MyProject", "2023.2.10f1")
		.WithExample("create", ".", "2023.2.10f1")
		.WithExample("create", "path/to/project", "2023.2.10f1");

	config.AddCommand<ProjectPathCommand>("project-path")
		.WithDescription("Get Unity project root directory from search path or via recent projects prompt")
		.WithExample("project-path")
		.WithExample("project-path", ".")
		.WithExample("project-path", "path/to/project")
		.WithExample("cd $(project-path)");

	config.AddCommand<InstallCommand>("install")
		.WithDescription("Install Unity Editor version")
		.WithExample("install", "2022.3.10f1")
		.WithExample("install", "2022.3.10f1", "ff3792e53c62")
		.WithExample("install", "2022.3.10f1", "--", "--module", "ios");

	config.AddCommand<EditorRevisionCommand>("editor-revision")
		.WithDescription("Get revision for Unity version")
		.WithExample("editor-revision", "2022.3.10f1");

	config.AddCommand<EditorPathCommand>("editor-path")
		.WithDescription("Get installation path for Unity version")
		.WithExample("editor-path", "2022.3.10f1");

	config.AddCommand<ProjectVersionCommand>("project-version")
		.WithDescription("Get Unity version from project search directory or ProjectVersion.txt")
		.WithExample("project-version", "path");

	config.AddCommand<VersionUsageCommand>("version-usage")
		.WithDescription("List installed Unity Editor versions and indicate which ones are used by projects")
		.WithExample("version-usage")
		.WithExample("version-usage", "--parseable");

	config.AddCommand<ProjectsUsingVersionCommand>("projects-using")
		.WithDescription("Find all projects that use a specific Unity version")
		.WithExample("projects-using", "2022.3.10f1");

	config.AddCommand<InstallMissingCommand>("install-missing")
		.WithDescription("Install all Unity versions that are used by projects but not currently installed")
		.WithExample("install-missing")
		.WithExample("install-missing", "--", "--module", "ios");

	config.AddCommand<UninstallUnusedCommand>("uninstall-unused")
		.WithDescription("Uninstall all Unity versions that are not used by any projects")
		.WithExample("uninstall-unused")
		.WithExample("uninstall-unused", "--dry-run");

	config.AddCommand<UpmGitUrlCommand>("upm-git-url")
		.WithDescription("Generate a git URL for Unity Package Manager from a Unity project")
		.WithExample("upm-git-url")
		.WithExample("upm-git-url", ".")
		.WithExample("upm-git-url", "path/to/project");

	config.AddCommand<HubCommand>("hub")
		.WithDescription("Execute Unity Hub interactively or with additional CLI arguments")
		.WithExample("hub")
		.WithExample("hub", "-- --headless help");
});

return app.Run(args);