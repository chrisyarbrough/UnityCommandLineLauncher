#!/bin/bash

# Opens a Unity project in the specified directory using the correct Unity Editor version.
# Usage: ./open_unity_project.sh [<project_directory>] [<additional_args>]
# If you pass additional args, you must provide the project_directory, 
# Example: ./open_unity_project.sh . -batchmode -quit

set -euo pipefail

PROJECT_DIR="${1:-$(pwd)}"

if [[ ! -d "$PROJECT_DIR" ]]; then
    echo "Error: Project directory '$PROJECT_DIR' is not a valid directory." >&2
    exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

PROJECT_VERSION_FILE=$(sh "$SCRIPT_DIR/get_unity_project_version_file.sh" "$PROJECT_DIR")
echo "File: $PROJECT_VERSION_FILE"

VERSION_AND_CHANGESET=$(sh "$SCRIPT_DIR/get_unity_project_version.sh" "$PROJECT_VERSION_FILE")
VERSION=${VERSION_AND_CHANGESET% *}
CHANGESET=${VERSION_AND_CHANGESET#* }
echo "Version: $VERSION ($CHANGESET)"

sh "$SCRIPT_DIR/ensure_unity_editor_install.sh" "$VERSION" "$CHANGESET"
UNITY_PATH=$(sh "$SCRIPT_DIR/get_unity_editor.sh" "$VERSION")
echo "Editor: $UNITY_PATH"

# Start Unity in background (don't block shell) and forward all arguments.
"$UNITY_PATH"/Contents/MacOS/Unity -projectPath "$PROJECT_DIR" "$@" &