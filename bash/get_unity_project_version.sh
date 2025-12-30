#!/bin/bash

# Extracts Unity version and changeset from ProjectVersion.txt
# Usage: ./get_unity_project_version.sh /path/to/ProjectVersion.txt
# Output: "2021.3.45f1 (0da89fac8e79)" or "2021.3.45f1"

set -euo pipefail

# Check if argument is provided
if [[ $# -lt 1 ]]; then
    echo "Usage: $0 /path/to/ProjectVersion.txt" >&2
    exit 1
fi

PROJECT_VERSION_FILE="$1"

# Check if file exists
if [[ ! -f "$PROJECT_VERSION_FILE" ]]; then
    echo "Error: File not found: $PROJECT_VERSION_FILE" >&2
    exit 2
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