#!/bin/bash

# Installs a specific Unity Editor version via Unity Hub if not already installed.
# Usage: ./unity-launcher-install.sh <version> [<changeset>]

if [ $# -lt 1 ]; then
    echo "Usage: $0 <version> [<changeset>]" >&2
    exit 1
fi

VERSION="$1"
SCRIPT_DIR="$(dirname "$0")"
source "$SCRIPT_DIR/utility.sh"
UNITY_HUB=$(get_unity_hub)

INSTALLED_EDITORS="$("$UNITY_HUB" -- --headless editors --installed 2>/dev/null)"
if echo "$INSTALLED_EDITORS" | grep -q "$VERSION"; then
    exit 0
fi

CHANGESET="$2"
if [ -z "$CHANGESET" ]; then
  echo "Changeset was not provided. Fetching from Unity API..."
  CHANGESET=$(sh "$SCRIPT_DIR/unity-launcher-editor-revision.sh" "$VERSION")
fi

echo "Installing Unity version $VERSION $CHANGESET..."
"$UNITY_HUB" -- --headless install --version "$VERSION" --changeset "$CHANGESET" --architecture "$(uname -m)" 2>/dev/null