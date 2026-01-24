# Unity Command Line Launcher (.NET)

## Prerequisites

- Check the [Directory.Build.props](../Directory.Build.props) file for the .NET SDK version to install:

```shell
sed -n 's/.*<TargetFramework>\(.*\)<\/TargetFramework>.*/\1/p' ../Directory.Build.props
```

- You should be familiar with using the `dotnet` CLI or the build/publish features in your IDE (e.g. JetBrains Rider).

---

## Building

The project requires no special process to compile the source. Simply run:

```shell
dotnet build
```

From the _ucll_ project directory.

To run the code without having to publish a standalone binary:

```shell
dotnet run -- --help
```

The first `--` separates the args passed to `dotnet run` from the args passed to ucll.

---

## Testing

Run the default test commands in the `ucll.tests` project:

```shell
dotnet test ../ucll.tests
```

---

## Debugging

This project uses only a single _Release_ configuration with debugging enabled.

Run with the `--debug` flag to enable timers and additional logs.
Run with the `--dry-run` flag to simulate mutating commands (e.g. open or install) without applying changes.

```shell
dotnet run -- version-usage --debug | grep -F "[Timing]"
```

---

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

---

## Testing

### Manual

See the [TestPlan](../../TestPlan.md). This plan can also be used as an inspiration for future automated tests.

### Virtual Machine

Manual platform testing via GUI can be performed by installing the tool within a virtual machine in, e.g.
[UTM](https://github.com/utmapp/UTM).

### Docker

Testing via CLI can be performed by running the tool in a container, e.g.:

```bash
brew install orbstack
```

```bash
orb start
```

```bash
dotnet publish --runtime linux-x64
docker run -it --rm --platform linux/amd64 \
-v ~/repos/opensource/UnityCommandLineLauncher/src/ucll/bin/linux-x64/publish:/app \
unityci/hub \
bash
```

```bash
 /app/ucll --help
 ```
