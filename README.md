# Unity Command Line Launcher

A terminal command designed to open Unity projects faster and more convenient than with the Unity Hub.

## Example

```bash
unity MyProject
```

or

```bash
cd MyProject
unity
```

![](docs/Screenshot.png)

## Features

- Opens Unity projects directly from the terminal without Unity Hub
- Automatically detects the correct Unity Editor version from the project
- Automatically installs missing Unity Editor versions via Unity Hub
- Fetches changesets from Unity API when not available locally
- Supports opening projects from current directory or by specifying a path
- Forwards additional Unity command line arguments (e.g., `-batchmode`, `-quit`)

## Support

Currently, only macOS is supported (tested with Sequoia 15.7.1).

## Setup

Create an alias in your shell config (.zshrc, .bashrc, etc.):

```bash
echo 'alias unity="~/path/to/UnityCommandLineLauncher/bash/open-unity.sh"' >> ~/.zshrc
```

## Design Background

### Problems

- Unity Hub is slow to open.
- Projects must be added to the Hub manually.
- Managing a large amount of projects can be cumbersome in a GUI workflow.
- Opening multiple test projects (e.g. when developing Unity packages) can be cumbersome.

### Solutions

- Developers have quick access to the Terminal:
    - Global hotkey in iTerm2.
    - Terminal window in IDE.
    - Context menu on directories in macOS Finder.
- Opening multiple projects can be automated with a helper script.

## Development

This repository provides multiple experimental implementations of the launcher tool.
All implementations are meant to provide the same functionality, and they are currently being evaluated.
In the end, a single implementation will be distributed as a package.
