package main

import (
	"fmt"
	"os/exec"
	"regexp"
	"runtime"
	"strings"
)

type EditorInfo struct {
	Version string
	Path    string
}

var hubPathCache string

// getUnityHubPath finds the Unity Hub executable path on macOS
func getUnityHubPath() (string, error) {
	if hubPathCache != "" {
		return hubPathCache, nil
	}

	cmd := exec.Command("mdfind", "kMDItemCFBundleIdentifier == 'com.unity3d.unityhub'")
	output, err := cmd.Output()
	if err != nil {
		return "", fmt.Errorf("failed to find Unity Hub: %w", err)
	}

	lines := strings.Split(strings.TrimSpace(string(output)), "\n")
	if len(lines) == 0 || lines[0] == "" {
		return "", fmt.Errorf("Unity Hub not found on this system")
	}

	hubPath := lines[0] + "/Contents/MacOS/Unity Hub"
	hubPathCache = hubPath
	return hubPath, nil
}

// listInstalledEditors returns a list of installed Unity editors
func listInstalledEditors() ([]EditorInfo, error) {
	hubPath, err := getUnityHubPath()
	if err != nil {
		return nil, err
	}

	cmd := exec.Command(hubPath, "--", "--headless", "editors", "--installed")
	output, err := cmd.Output()
	if err != nil {
		return nil, fmt.Errorf("failed to list installed editors: %w", err)
	}

	return parseEditorsOutput(string(output))
}

// parseEditorsOutput parses Unity Hub's editor list output
func parseEditorsOutput(output string) ([]EditorInfo, error) {
	var editors []EditorInfo

	// Pattern: 2021.3.45f1 (abc123) installed at /Applications/Unity/Hub/Editor/2021.3.45f1
	re := regexp.MustCompile(`([0-9]+\.[0-9]+\.[0-9]+[a-z0-9]*)\s+\(.*\)\s+installed\s+at\s+(.+)`)

	lines := strings.Split(output, "\n")
	for _, line := range lines {
		matches := re.FindStringSubmatch(line)
		if len(matches) == 3 {
			editors = append(editors, EditorInfo{
				Version: matches[1],
				Path:    matches[2],
			})
		}
	}

	return editors, nil
}

// getEditorPath returns the installation path for a specific Unity version
func getEditorPath(version string) (string, error) {
	editors, err := listInstalledEditors()
	if err != nil {
		return "", err
	}

	for _, editor := range editors {
		if editor.Version == version {
			return editor.Path, nil
		}
	}

	return "", fmt.Errorf("Unity version %s is not installed", version)
}

// isEditorInstalled checks if a specific Unity version is installed
func isEditorInstalled(version string) bool {
	editors, err := listInstalledEditors()
	if err != nil {
		return false
	}

	for _, editor := range editors {
		if editor.Version == version {
			return true
		}
	}

	return false
}

// installEditor installs a Unity Editor version via Unity Hub
func installEditor(version, changeset string) error {
	hubPath, err := getUnityHubPath()
	if err != nil {
		return err
	}

	arch := runtime.GOARCH
	if arch == "amd64" {
		arch = "x86_64"
	}

	logInfo("Installing Unity version %s %s...", version, changeset)

	cmd := exec.Command(hubPath, "--", "--headless", "install",
		"--version", version,
		"--changeset", changeset,
		"--architecture", arch)

	// Combine stdout and stderr
	output, err := cmd.CombinedOutput()
	if err != nil {
		return fmt.Errorf("failed to install Unity %s: %w\nOutput: %s", version, err, string(output))
	}

	return nil
}

// ensureEditorInstalled ensures a Unity Editor version is installed
func ensureEditorInstalled(version, changeset string) error {
	if isEditorInstalled(version) {
		return nil
	}

	if changeset == "" {
		logInfo("Changeset not provided, fetching from Unity API...")
		var err error
		changeset, err = fetchChangeset(version)
		if err != nil {
			return fmt.Errorf("failed to fetch changeset: %w", err)
		}
	}

	return installEditor(version, changeset)
}
