package main

import (
	"fmt"
	"os"
	"path/filepath"
)

const version = "1.0.0"

func printUsage() {
	fmt.Println("Unity Command Line Launcher")
	fmt.Println()
	fmt.Println("Usage:")
	fmt.Println("  unity [project_directory] [unity_arguments...]")
	fmt.Println()
	fmt.Println("Arguments:")
	fmt.Println("  project_directory    Path to Unity project (default: current directory)")
	fmt.Println("  unity_arguments      Additional arguments passed to Unity Editor")
	fmt.Println()
	fmt.Println("Options:")
	fmt.Println("  -h, --help          Show this help message")
	fmt.Println("  -v, --version       Show version information")
	fmt.Println()
	fmt.Println("Examples:")
	fmt.Println("  unity                    # Open project in current directory")
	fmt.Println("  unity /path/to/project   # Open project at specific path")
	fmt.Println("  unity . -batchmode -quit # Run in batch mode")
	fmt.Println("  unity . -force-metal     # Use Metal graphics API")
}

func printVersion() {
	fmt.Printf("Unity Launcher v%s\n", version)
}

func main() {
	// Handle help and version flags
	if len(os.Args) > 1 {
		switch os.Args[1] {
		case "-h", "--help":
			printUsage()
			os.Exit(0)
		case "-v", "--version":
			printVersion()
			os.Exit(0)
		}
	}

	// Determine project directory
	projectDir := "."
	additionalArgs := []string{}

	if len(os.Args) > 1 {
		projectDir = os.Args[1]
		if len(os.Args) > 2 {
			additionalArgs = os.Args[2:]
		}
	}

	// Validate project directory exists
	absProjectDir, err := filepath.Abs(projectDir)
	if err != nil {
		logError("Failed to resolve project directory: %v", err)
		os.Exit(1)
	}

	if info, err := os.Stat(absProjectDir); err != nil || !info.IsDir() {
		logError("Project directory '%s' is not a valid directory", projectDir)
		os.Exit(1)
	}

	// Find and parse project version
	projectVersion, err := getProjectVersion(absProjectDir)
	if err != nil {
		logError("%v", err)
		os.Exit(1)
	}

	changesetInfo := ""
	if projectVersion.Changeset != "" {
		changesetInfo = fmt.Sprintf(" (%s)", projectVersion.Changeset)
	}
	logInfo("Version: %s%s", projectVersion.Version, changesetInfo)

	// Ensure Unity Editor is installed
	if err := ensureEditorInstalled(projectVersion.Version, projectVersion.Changeset); err != nil {
		logError("%v", err)
		os.Exit(1)
	}

	// Get editor path
	editorPath, err := getEditorPath(projectVersion.Version)
	if err != nil {
		logError("%v", err)
		os.Exit(1)
	}

	logInfo("Editor: %s", editorPath)

	// Launch Unity
	if err := launchUnity(editorPath, absProjectDir, additionalArgs); err != nil {
		logError("%v", err)
		os.Exit(1)
	}
}
