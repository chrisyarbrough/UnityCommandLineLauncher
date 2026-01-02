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

- Opens Unity projects from the terminal
- Detects the correct Unity Editor version from the project
- Installs missing Unity Editor versions via Unity Hub
- Fetches changesets from Unity API when not available in ProjectVersion.txt (legacy projects)
- Forwards additional Unity command line arguments (e.g., `-batchmode`, `-quit`)

## Supported Platforms

- macOS (tested with Sequoia 15.7.1)

## Setup

Create an alias in your shell config (.zshrc, .bashrc, etc.):

```bash
echo 'alias unity="~/UnityCommandLineLauncher/dotnet/UnityLauncher/bin/Release/osx-arm64/publish/unity-launcher"' >> ~/.zshrc
```

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
