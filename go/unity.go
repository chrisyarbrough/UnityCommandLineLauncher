package main

import (
	"fmt"
	"os/exec"
	"path/filepath"
)

// launchUnity launches the Unity Editor with the specified project and arguments
func launchUnity(editorPath, projectPath string, additionalArgs []string) error {
	// Build the Unity executable path
	unityExec := filepath.Join(editorPath, "Contents", "MacOS", "Unity")

	// Build command arguments
	args := []string{"-projectPath", projectPath}
	args = append(args, additionalArgs...)

	// Create the command
	cmd := exec.Command(unityExec, args...)

	// Start Unity in the background (don't block)
	if err := cmd.Start(); err != nil {
		return fmt.Errorf("failed to launch Unity: %w", err)
	}

	logSuccess("Unity Editor launched successfully")

	// Don't wait for the process to finish
	return nil
}
