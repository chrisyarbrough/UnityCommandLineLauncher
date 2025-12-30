#!/bin/bash

if [ $# -lt 1 ]; then
    echo "Usage: $0 <version>" >&2
    exit 1
fi

VERSION="$1"
source "$(dirname "$0")/shared.sh"
UNITY_HUB=$(get_unity_hub)

while IFS= read -r line; do
    if [[ "$line" =~ ([0-9]+\.[0-9]+\.[0-9]+[a-z0-9]*)[[:space:]]+\(.*\)[[:space:]]+installed[[:space:]]+at[[:space:]]+(.+) ]]; then
        LINE_VERSION="${BASH_REMATCH[1]}"
        LINE_PATH="${BASH_REMATCH[2]}"
      
        if [[ "$LINE_VERSION" == "$VERSION" ]]; then
            echo "$LINE_PATH"
            exit 0
        fi
    else
        echo "Warning: Could not parse line: $line" >&2
    fi
done <<< "$("$UNITY_HUB" -- --headless editors --installed 2>/dev/null)"

echo "Error: Unity version $VERSION is not installed." >&2
exit 1