#!/bin/bash

# Opens a Unity project in the specified directory using the correct Unity Editor version.
# Usage: ./unity-launcher-open.sh <searchPath> [<additional_args>]
# Example: ./unity-launcher-open.sh . -batchmode -quit

set -euo pipefail

if [[ $# -lt 1 ]]; then
    echo "Usage: $0 <searchPath> [unityArgs...]" >&2
    exit 1
fi

SEARCH_PATH="$1"
shift  # Remove first argument, leaving remaining args for Unity

PROJECT_DIR=$(realpath "$SEARCH_PATH")

if [[ ! -d "$PROJECT_DIR" ]]; then
    echo "Error: Project directory '$SEARCH_PATH' is not a valid directory." >&2
    exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Find and display the ProjectVersion.txt file path
FOUND_FILES=$(find "$PROJECT_DIR" -path "*/ProjectSettings/ProjectVersion.txt" -type f 2>/dev/null || true)
if [ -n "$FOUND_FILES" ]; then
    PROJECT_VERSION_FILE=$(echo "$FOUND_FILES" | head -1)
    echo "File: $PROJECT_VERSION_FILE"
fi

VERSION_AND_CHANGESET=$(sh "$SCRIPT_DIR/unity-launcher-project-version.sh" "$PROJECT_DIR")
VERSION=${VERSION_AND_CHANGESET% *}
CHANGESET=${VERSION_AND_CHANGESET#* }
echo "Version: $VERSION ($CHANGESET)"

sh "$SCRIPT_DIR/unity-launcher-install.sh" "$VERSION" "$CHANGESET"
UNITY_PATH=$(sh "$SCRIPT_DIR/unity-launcher-editor-path.sh" "$VERSION")
echo "Editor: $UNITY_PATH"

# Start Unity in background (don't block shell) and forward all arguments.
"$UNITY_PATH"/Contents/MacOS/Unity -projectPath "$PROJECT_DIR" "$@" &