# Unity Command Line Launcher

A terminal command designed to open Unity projects quickly and with convenience.

![](docs/Screenshot_Open_Path.png)

## Supported Platforms

- macOS (tested with Sequoia 15.7.1)

## Commands

| Name                        | Description                                                          |
|-----------------------------|----------------------------------------------------------------------|
| `open [path]`               | Opens the Unity project.                                             |
| `open`                      | Shows a selection prompt of recent projects from the Unity Hub.      |
| `install <version>`         | Installs the editor by version, fetching the revision, if necessary. |
| `editor-revision <version>` | Fetches the revision from Unity's API.                               |
| `editor-path <version>`     | Gets the installation directory of the editor, if installed.         |
| `project-version <path>`    | Gets the Unity version information from a project.                   |

`path` can be:

- ProjectVersion.txt file
- A search directory to find a Unity project in (searches upwards and downwards)

---

## Setup

1. Download the binaries from the releases page or clone the repository.
2. Create an alias in your shell config (.zshrc, .bashrc, etc.):

```bash
echo 'alias unity="~/UnityCommandLineLauncher/dotnet/UnityLauncher/bin/Release/osx-arm64/publish/ucll"' >> ~/.zshrc
```

Or add the binary location to your PATH variable.

```bash
export PATH=$PATH:UnityLauncher/bin/Release/osx-arm64/publish/
```

### Enhanced Fuzzy Search (Optional)

Install [fzf](https://github.com/junegunn/fzf):

```bash
brew install fzf
```

With `fzf` installed, the interactive project selection (`unity open`) will use enhanced matching:

- **Acronyms**: `cfp` finds "core-frontend-platform"
- **Typo tolerance**: `sigle-sign` finds "single-sign-on"

If `fzf` is not installed, the built-in search will be used as a fallback.

![](docs/Screenshot_Open_Search.png)

---

## Usage

Discover available commands and options:

```bash
unity --help
```

Show more info about a command:

```bash
unity open --help
```

## Features

- Opens Unity projects from the terminal (faster than using the Unity Hub GUI)
- Interactive project selection from Unity Hub's recent projects (with optional favorite filter)
- Detects the Unity Editor version from the project
- Installs missing Unity Editor versions via Unity Hub
- Fetches changeset info from Unity API when missing in ProjectVersion.txt (e.g. legacy projects)
- Additional Unity command line arguments (e.g. `-batchmode -quit`) are forwarded
- Installation path auto-detection for Unity Hub and editors

## Configuration

Unity Hub and editor installations are detected in the following order:

1. Environment variables (optional, see table below)
2. Default paths on platform
3. Search heuristic to guess the paths

Setting the environment variables should not be necessary in most cases,
but it will speed up tool execution for non-default install locations.

| Platform | Environment Variable Example                                                            |
|----------|-----------------------------------------------------------------------------------------|
| macOS    | `UNITY_EDITOR_PATH="/Applications/Unity/Hub/Editor/{0}/Unity.app/Contents/MacOS/Unity"` |
|          | `UNITY_HUB_PATH="/Applications/Unity Hub.app/Contents/MacOS/Unity Hub"`                 |
|          |                                                                                         |
| Windows  | `UNITY_EDITOR_PATH="C:\Program Files\Unity\Hub\Editor\{0}\Editor\Unity.exe"`            |
|          | `UNITY_HUB_PATH="C:\Program Files\Unity Hub\Unity Hub.exe"`                             |
|          |                                                                                         |
| Linux    | `UNITY_EDITOR_PATH="/home/<user>/Unity/Hub/Editor/{0}/Editor/Unity"`                    |
|          | `UNITY_HUB_PATH="/home/<user>/Applications/Unity Hub.AppImage"`                         |

The placeholder `{0}` is part of the path pattern and will be replaced with the editor version at runtime.

## Design Background

### Problems

- Unity Hub is slow to open.
- Projects must be added to the Hub manually.
- Managing a large amount of projects can be cumbersome in a GUI workflow.
- Opening multiple test projects (e.g. when developing Unity packages) can be hard to automate.
- The Unity Hub CLI can be difficult to work with:
    - Hub path must be hardcoded or searched for with extra code.
    - Installing an editor requires passing the changeset in many cases, but the changeset is not always available in
      the ProjectVersion.txt file.
    - Installing an editor requires passing the architecture on macOS, which requires extra code to query this info.

### Solutions

- Developers have quick access to the Terminal:
    - Global hotkey in iTerm2.
    - Terminal window in IDE.
    - Context menu on directories in macOS Finder.
- Our custom tool addresses the Unity Hub API shortcommings by providing a streamlined UX.

## Development

This repository contains multiple implementation projects. The dotnet version is "officially" supported by me.
The additional implementations can be used as a reference or to copy-paste into your own projects.

## Outlook

If the tool receives enough stars/forks, I'll publish it to _Homebrew_ and similar package managers.
