#!/usr/bin/env python3

import os
import sys
import re
import subprocess
from pathlib import Path


def find_version(project_settings_path):
    # Extract the Unity version from the ProjectSettings file.
    version_pattern = re.compile(r'm_EditorVersion: (.*)')
    with open(project_settings_path, 'r') as file:
        for line in file:
            match = version_pattern.match(line)
            if match:
                return match.group(1)
    return None


def main():
    # Starting from the current working directory, search in multiple locations for the Unity ProjectVersion file.
    current_directory = Path.cwd()
    project_settings_file = Path('ProjectSettings') / 'ProjectVersion.txt'
    project_settings_paths = [
        current_directory / project_settings_file,
        current_directory / 'frontend' / project_settings_file,
        current_directory / '..' / project_settings_file
    ]

    project_settings_path = None

    for path in project_settings_paths:
        if path.is_file():
            project_settings_path = path.resolve()
            break

    if not project_settings_path:
        paths_string = '\n'.join(str(path.resolve()) for path in project_settings_paths)
        print(f"Couldn't find ProjectSettings file in:\n{paths_string}")
        sys.exit(1)

    unity_version = find_version(project_settings_path)

    if unity_version is None:
        print("Couldn't find Unity version in ProjectSettings file")
        sys.exit(2)

    unity_editor_path = f"/Applications/Unity/Hub/Editor/{unity_version}/Unity.app/Contents/MacOS/Unity"

    if not os.path.isfile(unity_editor_path):
        print(f"Couldn't find Unity Editor installation at {unity_editor_path}")
        sys.exit(3)

    # The project path is the one which contains the Assets, Library and ProjectSettings folders.
    project_path = str(project_settings_path.parent.parent)

    # Invoke the Unity Editor with the found project path and some global arguments.
    command = [
        unity_editor_path,
        '-projectPath', project_path,
        '-cacheServerEnableDownload', 'false',
        '-cacheServerEnableUpload', 'false',
    ]

    # Add any custom arguments passed to this script.
    command.extend(sys.argv[1:])

    print(f"Starting Unity {unity_version} with arguments: {' '.join(command[1:])}")

    # Start a new process which continues to live even after the shell is closed.
    subprocess.Popen(command)


if __name__ == '__main__':
    main()
