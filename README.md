# Unity Command Line Launcher

A terminal command designed to open Unity projects quickly and with convenience.
Builds on the Unity Hub CLI.

## Examples

```bash
unity open path/to/project
```

To see all commands:

```bash
unity --help
```

![](docs/Screenshot.png)

## Main Features

- Opens Unity projects from the terminal (faster than using the Unity Hub GUI)
- Detects the correct Unity Editor version from the project
- Installs missing Unity Editor versions via Unity Hub
- Fetches changeset info from Unity API when missing in ProjectVersion.txt (e.g. legacy projects)
- Forwards additional Unity command line arguments (e.g. `-batchmode -quit`)

## Commands

    open <searchPath>               Open Unity Editor for a project                                                    
    install <version>               Install Unity Editor version                                                       
    editor-revision <version>       Get revision for Unity version                                                     
    editor-path <version>           Get installation path for Unity version                                            
    project-version <searchPath>    Extract Unity version from project (search directory or path to ProjectVersion.txt)

## Supported Platforms

- macOS (tested with Sequoia 15.7.1)

## Setup

Create an alias in your shell config (.zshrc, .bashrc, etc.):

```bash
echo 'alias unity="~/UnityCommandLineLauncher/dotnet/UnityLauncher/bin/Release/osx-arm64/publish/unity-launcher"' >> ~/.zshrc
```

Or add the binary location to your PATH variable.

```bash
export PATH=$PATH:UnityLauncher/bin/Release/osx-arm64/publish/
```

## Configuration

The tool discovers Unity Hub and editor installations in the following order:

1. Environment variables (optional, see table below)
2. Default paths on platform
3. Search utility or similar heuristic to guess the paths

Setting the environment variables should not be necessary in most cases, but it will make tool execution quicker.

| Platform | Variable Example                                                                        |
|----------|-----------------------------------------------------------------------------------------|
| macOS    | `UNITY_EDITOR_PATH="/Applications/Unity/Hub/Editor/{0}/Unity.app/Contents/MacOS/Unity"` |
|          | `UNITY_HUB_PATH="/Applications/Unity Hub.app/Contents/MacOS/Unity Hub"`                 |
| Windows  | `UNITY_EDITOR_PATH="C:\Program Files\Unity\Hub\Editor\{0}\Editor\Unity.exe"`            |
|          | `UNITY_HUB_PATH="C:\Program Files\Unity Hub\Unity Hub.exe"`                             |
| Linux    | `UNITY_EDITOR_PATH="/home/<user>/Unity/Hub/Editor/{0}/Editor/Unity"`                    |
|          | `UNITY_HUB_PATH="/home/<user>/Applications/Unity Hub.AppImage"`                         |

The placeholder `{0}` is meant to be part of the path pattern and will be replaced with the editor version at runtime.

## Design Background

### Problems

- Unity Hub is slow to open.
- Projects must be added to the Hub manually.
- Managing a large amount of projects can be cumbersome in a GUI workflow.
- Opening multiple test projects (e.g. when developing Unity packages) can be cumbersome.
- The Unity Hub CLI can be cumbersome:
    - Hub path must be hardcoded or searched for with extra code.
    - Installing an editor requires passing the changeset in many cases, but the changeset is not always available in
      the ProjectVersion.txt file.
    - Installing an editor requires passing the architecture on macOS, which requires extra code to query this info.

### Solutions

- Developers have quick access to the Terminal:
    - Global hotkey in iTerm2.
    - Terminal window in IDE.
    - Context menu on directories in macOS Finder.
- Opening multiple projects can be automated with a helper script.
- Additional commands extend the Unity Hub CLI API.

## Development

This repository provides multiple experimental implementations of the launcher tool.
All implementations are meant to provide the same functionality, and they are currently being evaluated.
In the end, a single implementation will be distributed as a package.

Currently, the dotnet implementation is favored (and the most complete).
