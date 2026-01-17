# Unity Command Line Launcher (.NET)

## Debugging

This project uses only a single _Release_ configuration with debugging enabled.

Run with the `--debug` flag to enable timers and additional logs.
Run with the `--dry-run` flag to simulate mutating commands (e.g. open or install) without applying changes.

## Publishing

Run the `uccl.build` to create signed release artifacts:

```bash
dotnet run --project ../ucll.build
```

The binaries are configured in the .csproj to be self-contained
(they don't require a .NET runtime installed by the user).

## Dockerized Testing

There's no pipeline set up yet, but we can do some manual platform testing like this:

```bash
brew install orbstack
```

```bash
orb start
```

```bash
dotnet publish --runtime linux-x64
docker run -it --rm --platform linux/amd64 \
-v /Users/christopher.yarbrough/repos/opensource/UnityCommandLineLauncher/dotnet/ucll/bin/linux-x64/publish:/app \
unityci/hub \
bash
```

```bash
 /app/ucll --help
 ```
