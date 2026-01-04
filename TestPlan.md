# Test Plan

This document contains test cases that need to be verified before releasing a new version.

## Test Cases

### `open`

| Test Case                                | Input                                                    | Expected Outcome                                                | Pass? |
|------------------------------------------|----------------------------------------------------------|-----------------------------------------------------------------|:-----:|
| Valid project path                       | `unity open /path/to/project` (with installed version)   | Editor launches successfully with project                       |  [ ]  |
| Valid project path (upward search)       | `unity open /project/Assets`                             | Editor launches successfully with project                       |  [ ]  |
| Valid project path (downward search)     | `unity open /repo-root`                                  | Editor launches successfully with project                       |  [ ]  |
| Valid project with current directory     | `unity open .`                                           | Editor launches successfully with current directory as project  |  [ ]  |
| Invalid project path                     | `unity open /path/to/nonexistent`                        | Shows error                                                     |  [ ]  |
| Project with additional Unity args       | `unity open /path/to/project -- -batchmode -quit`        | Editor launches with additional arguments passed through        |  [ ]  |
| Project missing ProjectVersion.txt       | `unity open /path/to/project/without/version`            | Shows error                                                     |  [ ]  |
| Project with Unity version not installed | `unity open /path/to/project` (with uninstalled version) | Attempts to install version, then launches editor               |  [ ]  |
| Interactive selection                    | `unity open /path/to/repos` (multiple projects)          | Shows interactive selection prompt with found projects          |  [ ]  |
| Interactive selection (recent projects)  | `unity open` (with recent projects in Unity Hub)         | Shows interactive selection prompt with recent projects         |  [ ]  |
| Interactive selection with search        | `unity open` → type search term (with fzf installed)     | Filters projects by search term with fuzzy matching             |  [ ]  |
| Interactive selection with search        | `unity open` → type search term (fallback)               | Filters projects by search term with substring matching         |  [ ]  |
| Interactive selection - no projects      | `unity open` (no recent projects in Unity Hub)           | Shows error                                                     |  [ ]  |
| Interactive favorites only               | `unity open --favorite` (with favorite projects)         | Shows only favorite projects in selection prompt (still sorted) |  [ ]  |
| Interactive favorites - no favorites     | `unity open --favorite` (no favorites marked)            | Shows error                                                     |  [ ]  |
| Favorites flag alias (--favorite)        | `unity open --favorites`                                 | Alias for --favorite                                            |  [ ]  |

### `install`

| Test Case                      | Input                                           | Expected Outcome                                              | Pass? |
|--------------------------------|-------------------------------------------------|---------------------------------------------------------------|:-----:|
| Valid Unity version            | `unity install 2022.3.10f1`                     | Unity version is installed successfully, revision was fetched |  [ ]  |
| Valid version with revision    | `unity install 2022.3.10f1 ff3792e53c62`        | Unity version is installed successfully                       |  [ ]  |
| Invalid Unity version format   | `unity install invalid.string`                  | Shows error                                                   |  [ ]  |
| Non-existent Unity version     | `unity install 9999.9.9f1`                      | Shows error                                                   |  [ ]  |
| Already installed version      | `unity install 2022.3.10f1` (already installed) | Success message shown (idempotent operation)                  |  [ ]  |
| Install with Unity Hub modules | `unity install 2022.3.10f1 -- --module webgl`   | Installs with additional Hub arguments passed through         |  [ ]  |

### `editor-revision`

| Test Case                    | Input                                   | Expected Outcome          | Pass? |
|------------------------------|-----------------------------------------|---------------------------|:-----:|
| Valid Unity version          | `unity editor-revision 2022.3.10f1`     | Outputs the revision hash |  [ ]  |
| Invalid Unity version format | `unity editor-revision invalid.version` | Shows error               |  [ ]  |
| Non-existent Unity version   | `unity editor-revision 9999.9.9f1`      | Shows error               |  [ ]  |

### `editor-path`

| Test Case                    | Input                                          | Expected Outcome                                | Pass? |
|------------------------------|------------------------------------------------|-------------------------------------------------|:-----:|
| Installed Unity version      | `unity editor-path 2022.3.10f1`                | Outputs the full path to installed Unity editor |  [ ]  |
| Non-installed Unity version  | `unity editor-path 2023.1.1f1` (not installed) | Shows error                                     |  [ ]  |
| Invalid Unity version format | `unity editor-path invalid`                    | Shows error                                     |  [ ]  |

### `project-version`

| Test Case                            | Input                                         | Expected Outcome                                           | Pass? |
|--------------------------------------|-----------------------------------------------|------------------------------------------------------------|:-----:|
| Valid Unity project directory        | `unity project-version /path/to/project`      | Outputs version (and revision if present)                  |  [ ]  |
| Current directory with project       | `unity project-version .`                     | Outputs version from current directory's project           |  [ ]  |
| Directory without ProjectVersion.txt | `unity project-version /path/without/version` | Shows error                                                |  [ ]  |
| Invalid directory                    | `unity project-version /nonexistent/path`     | Shows error                                                |  [ ]  |
| Project with version only            | `unity project-version /path/to/project`      | Outputs version only (e.g. "2022.3.10f1")                  |  [ ]  |
| Project with version and revision    | `unity project-version /path/to/project`      | Outputs version and revision (e.g. "2022.3.10f1 abcd1234") |  [ ]  |

### General

| Test Case           | Input                         | Expected Outcome                           | Pass? |
|---------------------|-------------------------------|--------------------------------------------|:-----:|
| No command provided | `unity`                       | Shows help/usage information               |  [ ]  |
| Help flag           | `unity --help`                | Shows help/usage information               |  [ ]  |
| Version flag        | `unity --version`             | Shows version                              |  [ ]  |
| Command validation  | `unity install` (missing arg) | All commands should validate required args |  [ ]  |

- Commands use Spectre.Console for output formatting.
- All `path` arguments can also be search directories outside or inside of Unity projects.

Manually check installed editors:

```bash
'/Applications/Unity Hub.app/Contents/MacOS/Unity Hub' -- --headless editors --installed
```

## Known Issues

1. Opening a project via our tool might not update the Unity Hub recent projects list immediately (restart required).
2. Exiting the Spectre.Console SelectionPrompt via CTRL + C leaves the cursor in a hidden state.