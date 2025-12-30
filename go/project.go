package main

import (
	"fmt"
	"os"
	"path/filepath"
	"regexp"
	"strings"
)

type UnityVersion struct {
	Version   string
	Changeset string
}

// findProjectVersionFile finds the ProjectVersion.txt file in a directory tree
func findProjectVersionFile(searchDir string) (string, error) {
	var foundFiles []string

	err := filepath.Walk(searchDir, func(path string, info os.FileInfo, err error) error {
		if err != nil {
			return nil // Skip directories we can't read
		}

		// Check if this is the ProjectVersion.txt file we're looking for
		if !info.IsDir() && filepath.Base(path) == "ProjectVersion.txt" {
			// Ensure it's in a ProjectSettings directory
			if filepath.Base(filepath.Dir(path)) == "ProjectSettings" {
				absPath, err := filepath.Abs(path)
				if err == nil {
					foundFiles = append(foundFiles, absPath)
				}
			}
		}

		return nil
	})

	if err != nil {
		return "", fmt.Errorf("error searching for ProjectVersion.txt: %w", err)
	}

	if len(foundFiles) == 0 {
		return "", fmt.Errorf("no ProjectSettings/ProjectVersion.txt found in %s", searchDir)
	}

	if len(foundFiles) > 1 {
		return "", fmt.Errorf("found multiple ProjectVersion.txt files:\n%s\nPlease run in a directory with only one Unity project",
			strings.Join(foundFiles, "\n"))
	}

	return foundFiles[0], nil
}

// parseProjectVersion extracts Unity version and changeset from ProjectVersion.txt
func parseProjectVersion(filePath string) (*UnityVersion, error) {
	content, err := os.ReadFile(filePath)
	if err != nil {
		return nil, fmt.Errorf("failed to read %s: %w", filePath, err)
	}

	text := string(content)

	// Try pattern 1: m_EditorVersionWithRevision: 2021.3.45f1 (abc123)
	re1 := regexp.MustCompile(`m_EditorVersionWithRevision:\s+(.+)\s+\((.+)\)`)
	if matches := re1.FindStringSubmatch(text); len(matches) == 3 {
		return &UnityVersion{
			Version:   strings.TrimSpace(matches[1]),
			Changeset: strings.TrimSpace(matches[2]),
		}, nil
	}

	// Try pattern 2: m_EditorVersion: 2021.3.45f1
	re2 := regexp.MustCompile(`m_EditorVersion:\s+(.+)`)
	if matches := re2.FindStringSubmatch(text); len(matches) == 2 {
		return &UnityVersion{
			Version:   strings.TrimSpace(matches[1]),
			Changeset: "",
		}, nil
	}

	return nil, fmt.Errorf("could not find Unity version in %s", filePath)
}

// getProjectVersion finds and parses the Unity project version
func getProjectVersion(projectDir string) (*UnityVersion, error) {
	versionFile, err := findProjectVersionFile(projectDir)
	if err != nil {
		return nil, err
	}

	logInfo("File: %s", versionFile)

	version, err := parseProjectVersion(versionFile)
	if err != nil {
		return nil, err
	}

	return version, nil
}
