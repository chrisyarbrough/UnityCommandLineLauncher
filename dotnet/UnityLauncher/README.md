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

## Testing Helpers

```bash
brew install orbstack
```

```bash
docker run -it --rm --platform linux/amd64 \
-v /Users/christopher.yarbrough/repos/opensource/UnityCommandLineLauncher/dotnet/UnityLauncher/bin/linux-x64/publish:/app \
unityci/hub \
bash
```

```bash
 /app/unity-launcher --help
 ```

```bash
'/Applications/Unity Hub.app/Contents/MacOS/Unity Hub' -- --headless editors --installed
```

