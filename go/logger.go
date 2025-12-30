package main

import (
	"fmt"
	"os"
)

// ANSI color codes
const (
	colorReset  = "\033[0m"
	colorRed    = "\033[31m"
	colorGreen  = "\033[32m"
	colorYellow = "\033[33m"
	colorCyan   = "\033[36m"
)

func isTerminal() bool {
	fileInfo, _ := os.Stdout.Stat()
	return (fileInfo.Mode() & os.ModeCharDevice) != 0
}

func colorize(color, text string) string {
	if isTerminal() {
		return color + text + colorReset
	}
	return text
}

func logInfo(format string, args ...interface{}) {
	fmt.Println(colorize(colorCyan, fmt.Sprintf(format, args...)))
}

func logSuccess(format string, args ...interface{}) {
	fmt.Println(colorize(colorGreen, fmt.Sprintf(format, args...)))
}

func logError(format string, args ...interface{}) {
	fmt.Fprintln(os.Stderr, colorize(colorRed, "Error: "+fmt.Sprintf(format, args...)))
}

func logWarning(format string, args ...interface{}) {
	fmt.Fprintln(os.Stderr, colorize(colorYellow, "Warning: "+fmt.Sprintf(format, args...)))
}
