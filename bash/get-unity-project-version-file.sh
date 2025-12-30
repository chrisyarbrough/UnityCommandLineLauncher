#!/bin/bash

# Finds Unity ProjectVersion.txt files in a directory tree.
# Set the current directory to the search root or pass it as a first argument.

set -euo pipefail

SEARCH_DIR="${1:-.}"

if [[ ! -d "$SEARCH_DIR" ]]; then
    echo "Error: '$SEARCH_DIR' is not a directory" >&2
    exit 1
fi

FOUND_FILES=$(find "$SEARCH_DIR" -path "*/ProjectSettings/ProjectVersion.txt" -type f 2>/dev/null || true)

FILE_COUNT=0
if [ -n "$FOUND_FILES" ]; then
    FILE_COUNT=$(echo "$FOUND_FILES" | wc -l | tr -d ' ')
fi

if [ "$FILE_COUNT" -eq 0 ]; then
    echo "Error: No ProjectSettings/ProjectVersion.txt found" >&2
    exit 1
elif [ "$FILE_COUNT" -gt 1 ]; then
    echo "Error: Found too many ProjectVersion.txt files:" >&2
    echo "${FOUND_FILES}"
    echo "Please run the script in a directory with only one Unity project."
    exit 2
else
    # Get absolute path
    FILE=$(echo "$FOUND_FILES" | head -1)
    realpath "$FILE"
fi