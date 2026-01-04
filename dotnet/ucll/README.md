### Publishing

```bash
dotnet publish --runtime osx-arm64
dotnet publish --runtime osx-x64
```

```bash
dotnet publish --runtime linux-x64
dotnet publish --runtime win-x64
```

This project uses only a single _Release_ configuration with debugging enabled.

Add the `--debug` flag to enable timers and additional logs.
Add the `--dry-run` flag to run mutating commands without applying changes (e.g. open or install).

## Dockerized Testing

There's no pipeline set up yet, but we can do some manually testing like this:

```bash
brew install orbstack
```

```bash
orb start
```

```bash
docker run -it --rm --platform linux/amd64 \
-v ~/repos/opensource/UnityCommandLineLauncher/dotnet/UnityLauncher/bin/linux-x64/publish:/app \
unityci/hub \
bash
```

```bash
 /app/ucll --help
 ```
