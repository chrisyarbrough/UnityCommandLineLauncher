#!/bin/bash

# Extracts Unity version and changeset from ProjectVersion.txt
# Usage: ./unity-launcher-project-version.sh <searchPath>
# searchPath can be either a directory containing a Unity project or a direct path to ProjectVersion.txt
# Output: "2021.3.45f1 0da89fac8e79" or "2021.3.45f1"

set -euo pipefail

# Check if argument is provided
if [[ $# -lt 1 ]]; then
    echo "Usage: $0 <searchPath>" >&2
    exit 1
fi

SEARCH_PATH="$1"

# Determine if searchPath is a file or directory
if [[ -f "$SEARCH_PATH" ]]; then
    PROJECT_VERSION_FILE="$SEARCH_PATH"
elif [[ -d "$SEARCH_PATH" ]]; then
    # Find ProjectVersion.txt in directory
    FOUND_FILES=$(find "$SEARCH_PATH" -path "*/ProjectSettings/ProjectVersion.txt" -type f 2>/dev/null || true)

    FILE_COUNT=0
    if [ -n "$FOUND_FILES" ]; then
        FILE_COUNT=$(echo "$FOUND_FILES" | wc -l | tr -d ' ')
    fi

    if [ "$FILE_COUNT" -eq 0 ]; then
        echo "Error: No ProjectSettings/ProjectVersion.txt found in $SEARCH_PATH" >&2
        exit 2
    elif [ "$FILE_COUNT" -gt 1 ]; then
        echo "Error: Found multiple ProjectVersion.txt files:" >&2
        echo "${FOUND_FILES}" >&2
        echo "Please run in a directory with only one Unity project." >&2
        exit 2
    fi

    PROJECT_VERSION_FILE=$(echo "$FOUND_FILES" | head -1)
else
    echo "Error: Argument must be a valid directory or ProjectVersion.txt file path." >&2
    exit 1
fi

# Read entire file into variable
CONTENT=$(<"$PROJECT_VERSION_FILE")

# First try: m_EditorVersionWithRevision: version (changeset)
if [[ "$CONTENT" =~ m_EditorVersionWithRevision:[[:space:]]+(.+)[[:space:]]+\((.+)\) ]]; then
    echo "${BASH_REMATCH[1]} ${BASH_REMATCH[2]}"
    
# Second try: m_EditorVersion: version
elif [[ "$CONTENT" =~ m_EditorVersion:[[:space:]]+(.+) ]]; then
    echo "${BASH_REMATCH[1]}"

else
    echo "Error: Could not find Unity version in file." >&2
    exit 3
fi