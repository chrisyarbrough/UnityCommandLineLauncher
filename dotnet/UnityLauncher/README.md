### Publishing

```bash
dotnet publish --runtime osx-arm64
dotnet publish --runtime osx-x64
```

```bash
dotnet publish --runtime linux-x64
dotnet publish --runtime win-x64
```

Note: Non-macOS platforms will require platform-specific code changes (Unity Hub lookup, etc.)
