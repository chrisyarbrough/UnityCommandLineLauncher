#!/bin/bash

if [ $# -lt 1 ]; then
    echo "Usage: $0 <version>" >&2
    exit 1
fi

VERSION="$1"
UNITY_HUB="$(mdfind "kMDItemCFBundleIdentifier == 'com.unity3d.unityhub'" | head -n 1)/Contents/MacOS/Unity Hub"

"$UNITY_HUB" -- --headless editors --installed 2>/dev/null | while IFS= read -r line; do
    # (\d+\.\d+\.\d+[abfp]\d+)\s+.*installed at (.+)
    if [[ "$line" =~ ([0-9]+\.[0-9]+\.[0-9]+[a-z0-9]*)[[:space:]]+\(.*\)[[:space:]]+installed[[:space:]]+at[[:space:]]+(.+) ]]; then
        LINE_VERSION="${BASH_REMATCH[1]}"
        LINE_PATH="${BASH_REMATCH[2]}"
      
        if [[ "$LINE_VERSION" == "$VERSION" ]]; then
            echo "$LINE_PATH"
        fi
    else
        echo "Warning: Could not parse line: $line" >&2
    fi
done