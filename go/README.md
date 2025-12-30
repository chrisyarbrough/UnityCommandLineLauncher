# Unity Command Line Launcher (Go)

A lightweight Go implementation of the Unity Command Line Launcher. Opens Unity projects directly from the command line, bypassing Unity Hub for faster project loading.

## Features

- 🔍 Automatically finds Unity project in current directory tree
- 📦 Parses project version from `ProjectSettings/ProjectVersion.txt`
- 🚀 Auto-installs missing Unity Editor versions via Unity Hub
- 🎨 Colored terminal output
- ⚡ Fast and efficient (single binary, no dependencies)
- 🔧 Forwards additional arguments to Unity Editor

## Requirements

- macOS (currently only supports macOS)
- Unity Hub must be installed
- Go 1.21+ (for building from source)

## Installation

### Option 1: Build and Install

```bash
cd go
make install
```

This will build the binary and install it to `~/bin/unity`. Make sure `~/bin` is in your PATH.

### Option 2: Build Only

```bash
cd go
make build
```

Then manually copy the `unity` binary to a directory in your PATH.

### Option 3: Manual Build

```bash
cd go
go build -o unity .
```

## Usage

Navigate to any directory within your Unity project or the project root, then run:

```bash
unity
```

### Examples

Open project in current directory:
```bash
unity
```

Open specific project directory:
```bash
unity /path/to/unity/project
```

Pass additional arguments to Unity:
```bash
unity . -batchmode -quit
unity . -force-metal
```

Show help:
```bash
unity --help
```

Show version:
```bash
unity --version
```

## How It Works

1. **Project Discovery**: Searches for `ProjectSettings/ProjectVersion.txt` in the directory tree
2. **Version Parsing**: Extracts Unity version and changeset from the project file
3. **Installation Check**: Verifies if the required Unity Editor version is installed
4. **Auto-Install**: If not installed, fetches changeset (if missing) and installs via Unity Hub
5. **Launch**: Starts Unity Editor with the project path and any additional arguments

## Project Structure

```
go/
├── main.go      # CLI entry point and argument parsing
├── logger.go    # Colored terminal output
├── hub.go       # Unity Hub integration (locator, editor list, installer)
├── project.go   # Project finder and version parser
├── api.go       # Unity API client (fetch changeset)
├── unity.go     # Unity Editor launcher
├── Makefile     # Build automation
└── README.md    # This file
```

## Building

The project uses standard Go tooling:

```bash
# Build
go build -o unity .

# Build with optimizations
go build -ldflags="-s -w" -o unity .

# Cross-compile (example)
GOOS=darwin GOARCH=arm64 go build -o unity-arm64 .
```

## Makefile Targets

- `make build` - Build the binary
- `make install` - Build and install to ~/bin
- `make clean` - Remove built binary
- `make help` - Show available targets

## Configuration

No configuration file is needed. The tool automatically:
- Finds Unity Hub using `mdfind`
- Queries Unity Hub for installed editors
- Uses Unity's public API for missing changeset information

## Troubleshooting

**Error: Unity Hub not found**
- Ensure Unity Hub is installed and accessible via Spotlight (`mdfind`)

**Error: No ProjectSettings/ProjectVersion.txt found**
- Run the command from within a Unity project directory
- Ensure the project has a valid `ProjectSettings/ProjectVersion.txt` file

**Error: Unity version not installed**
- The tool will attempt to auto-install
- Ensure Unity Hub is running and you have internet connectivity
- Check Unity Hub logs if installation fails

## Comparison with Bash Version

| Feature | Bash | Go |
|---------|------|-----|
| Speed | Fast | Faster (single binary) |
| Dependencies | bash, jq, curl | None (single binary) |
| Platform Support | macOS only | macOS (expandable to other platforms) |
| Error Messages | Basic | Detailed with colors |
| Code Organization | Multiple scripts | Single binary |
| Binary Size | N/A | ~8MB |

## Future Enhancements

- [ ] Windows support (Registry-based Unity Hub lookup)
- [ ] Linux support (standard paths)
- [ ] Configuration file support (`~/.unity-launcher.yaml`)
- [ ] Dry-run mode (`--dry-run`)
- [ ] Progress indicators for installations
- [ ] Multiple project handling with interactive selection

## License

See the LICENSE file in the root of this repository.
