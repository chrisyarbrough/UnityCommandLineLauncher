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

	throw new PlatformNotSupportedException("Unsupported platform.");
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

	config.AddCommand<InstallationsOverviewCommand>("installations-all")
		.WithDescription("List installed Unity Editor versions")
		.WithExample("installations-all")
		.WithExample("installations-all", "--parseable");

	config.AddCommand<InstallationsUsedCommand>("installations-used")
		.WithDescription("Find all projects that use a specific Unity version")
		.WithExample("installations-used", "2022.3.10f1");

	config.AddCommand<InstallationsInstallMissingCommand>("installations-install-missing")
		.WithDescription("Install all Unity versions that are used by projects but not currently installed")
		.WithExample("installations-install-missing")
		.WithExample("installations-install-missing", "--", "--module", "ios");

	config.AddCommand<InstallationsUninstallUnusedCommand>("installations-cleanup")
		.WithDescription("Uninstall all Unity versions that are not used by any projects")
		.WithExample("installations-cleanup")
		.WithExample("installations-cleanup", "--dry-run");
});

return app.Run(args);

public sealed class TypeRegistrar(IServiceCollection services) : ITypeRegistrar
{
	public ITypeResolver Build() => new TypeResolver(services.BuildServiceProvider());

	public void Register(Type service, Type implementation) => services.AddSingleton(service, implementation);

	public void RegisterInstance(Type service, object implementation) => services.AddSingleton(service, implementation);

	public void RegisterLazy(Type service, Func<object> factory) => services.AddSingleton(service, _ => factory());
}

public sealed class TypeResolver(IServiceProvider provider) : ITypeResolver
{
	public object? Resolve(Type? type) => type == null ? null : provider.GetService(type);
}