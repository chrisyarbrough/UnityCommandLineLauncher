# Unity Command Line Launcher (.NET)

## Prerequisites

- Check the [Directory.Build.props](../Directory.Build.props) file for the .NET version to install.
- You should be familiar with using the `dotnet` CLI or the build/publish features in your IDE (e.g. JetBrains Rider).

## Building

The project requires no special process to compile the source. Simply run:

```shell
dotnet build
```

From the project directory `ucll`.

You can then run the code without having to publish a standalone binary via:

```shell
dotnet run
```

## Testing

Run the default test commands in the `ucll.tests` project:

```shell
dotnet test
```

## Debugging

This project uses only a single _Release_ configuration with debugging enabled.

Run with the `--debug` flag to enable timers and additional logs.
Run with the `--dry-run` flag to simulate mutating commands (e.g. open or install) without applying changes.

## Publishing

If you want to publish (export) a single-file self-contained binary for your current platform, run:

```shell
dotnet publish
```

For the `ucll` project. You will find the build output e.g. here: `src/ucll/bin/osx-arm64/publish/ucll`

If you want to publish releases for all platforms and sign them, **run** the `ucll.build` project:

```bash
dotnet run --project ../ucll.build
```

> Calling `dotnet publish --project ../ucll.build` would publish the build project itself, which wouldn't make sense.

The binaries are configured in the .csproj to be self-contained
(they don't require a .NET runtime installed by the user).

## Virtual Machine Testing

Currently, I perform manual tests by running a virtual machine in [UTM](https://github.com/utmapp/UTM).

## Dockerized Testing

There's no automated pipeline set up yet, but we can also do some manual platform testing like this:

```bash
brew install orbstack
```

```bash
orb start
```

```bash
dotnet publish --runtime linux-x64
docker run -it --rm --platform linux/amd64 \
-v /Users/christopher.yarbrough/repos/opensource/UnityCommandLineLauncher/src/ucll/bin/linux-x64/publish:/app \
unityci/hub \
bash
```

```bash
 /app/ucll --help
 ```
