#!/bin/bash

# Installs a specific Unity Editor version via Unity Hub if not already installed.
# Usage: ./ensure_unity_editor_install.sh <version> [<changeset>]

if [ $# -lt 1 ]; then
    echo "Usage: $0 <version> [<changeset>]" >&2
    exit 1
fi

VERSION="$1"
UNITY_HUB="$(mdfind "kMDItemCFBundleIdentifier == 'com.unity3d.unityhub'" | head -n 1)/Contents/MacOS/Unity Hub"
INSTALLED_EDITORS="$("$UNITY_HUB" -- --headless editors --installed 2>/dev/null)"
if echo "$INSTALLED_EDITORS" | grep -q "$VERSION"; then
    exit 0
fi

CHANGESET="$2"
if [ -z "$CHANGESET" ]; then
  echo "Changeset was not provided. Fetching from Unity API..."
  CHANGESET=$(curl -s "https://services.api.unity.com/unity/editor/release/v1/releases?version=$VERSION" | jq -r '.results[0].shortRevision')
fi

echo "Installing Unity version $VERSION $CHANGESET..."
"$UNITY_HUB" -- --headless install --version "$VERSION" --changeset "$CHANGESET" --architecture "$(uname -m)" 2>/dev/null