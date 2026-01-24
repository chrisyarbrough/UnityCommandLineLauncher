using Microsoft.Extensions.DependencyInjection;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var services = new ServiceCollection();
services.AddSingleton(PlatformSupport.Create());
services.AddSingleton<UnityHub>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);
app.Configure(AppConfiguration.Build);
return app.Run(args);