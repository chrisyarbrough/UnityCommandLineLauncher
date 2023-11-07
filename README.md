# Unity Command Line Launcher

A lightweight command-line script designed to speedup opening Unity projects. With a single command, this script launches a Unity project directly from within the project's directory tree, bypassing the Unity Hub for enhanced speed and convenience. It's tailored for developers who prefer the efficiency of a terminal-based approach.

Recommended workflow:
1) Open your Unity project in your IDE from the recent projects list
2) Use a keyboard shortcut to open the command line window in the open Unity project working directory
3) Use the launcher command to open the Unity project

This approach is much quicker than:
1) Waiting for the slow Unity Hub to start
2) Selecting the Unity project in the list and waiting for it to open
3) Double-clicking a script or selecting _Assets > Open C# Project_ in Unity to open the IDE

# Support

The script currently only supports macOS and was tested on Ventura 13.4.

# Setup
- Install Python 3
- Place the launcher script anywhere on your machine
- Create a globally available command alias that points to the script. For example, for the ZSH shell, add `alias unity="~/bin/open-unity.py"` to the `.zshrc` file.

```bash
echo 'alias unity="~/bin/open-unity.py"' >> ~/.zshrc
```

Alternatively, rename the script to _unity_ and place it in a directory that is part of the PATH shell variable.

# Usage
Open your IDE (e.g. Rider or Visual Studio) with the desired Unity project or any command line session in one of the following directories:
- The Unity project root directory (which contains Assets, Library and ProjectSettings)
- Any direct child directory (e.g. Assets, Library, ProjectSettings)
- A directory which contains the directory "frontend" which is the Unity project root

Invoke the `unity` command alias to start Unity and open the current project:

```bash
unity
```

Pass additional arguments via the command:

```bash
unity -force-metal
```
