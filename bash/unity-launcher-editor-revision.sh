#!/bin/bash

# Get the revision (changeset) for a Unity Editor version from the Unity API.
# Usage: ./unity-launcher-editor-revision.sh <version>

if [ $# -lt 1 ]; then
    echo "Usage: $0 <version>" >&2
    exit 1
fi

VERSION="$1"

CHANGESET=$(curl -s "https://services.api.unity.com/unity/editor/release/v1/releases?version=$VERSION" | jq -r '.results[0].shortRevision')

if [ -z "$CHANGESET" ] || [ "$CHANGESET" = "null" ]; then
    echo "Error: Could not fetch changeset for version $VERSION" >&2
    exit 1
fi

echo "$CHANGESET"