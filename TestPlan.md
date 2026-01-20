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
| Interactive selection                    | `unity open /path/to/repos` (multiple projects)          | Shows interactive selection prompt with found projects          |  [ ]  |
| Invalid project path                     | `unity open /path/to/nonexistent`                        | Shows error                                                     |  [ ]  |
| Project with additional Unity args       | `unity open /path/to/project -- -batchmode -quit`        | Editor launches with additional arguments passed through        |  [ ]  |
| Project with Unity version not installed | `unity open /path/to/project` (with uninstalled version) | Attempts to install version, then launches editor               |  [ ]  |
| Interactive selection (recent projects)  | `unity open` (with recent projects in Unity Hub)         | Shows interactive selection prompt with recent projects         |  [ ]  |
| Interactive selection with search        | `unity open` → type search term (with fzf installed)     | Filters projects by search term with fuzzy matching             |  [ ]  |
| Interactive selection with search        | `unity open` → type search term (fallback)               | Filters projects by search term with substring matching         |  [ ]  |
| Interactive selection - no projects      | `unity open` (no recent projects in Unity Hub)           | Shows error                                                     |  [ ]  |
| Interactive favorites only               | `unity open --favorite` (with favorite projects)         | Shows only favorite projects in selection prompt (still sorted) |  [ ]  |
| Interactive favorites - no favorites     | `unity open --favorite` (no favorites marked)            | Shows error                                                     |  [ ]  |
| Favorites flag alias (--favorite)        | `unity open --favorites`                                 | Alias for --favorite                                            |  [ ]  |
| Open with code editor                    | `unity open path --code-editor`                          | Opens project's solution file in default code editor            |  [ ]  |
| Open with code editor (no solution)      | `unity open path --code-editor` (not yet generated)      | Opens project's solution file as soon as it is generated        |  [ ]  |
| Open with dry-run                        | `unity open path --dry-run`                              | Shows what would be executed without opening                    |  [ ]  |

### `create`

| Test Case                       | Input                                                                     | Expected Outcome                                 | Pass? |
|---------------------------------|---------------------------------------------------------------------------|--------------------------------------------------|:-----:|
| Create project with default     | `unity create /path/to/new/project`                                       | Prompts for version and creates                  |  [ ]  |
| Create project with version     | `unity create /path/to/new/project 2022.3.10f1`                           | Creates new Unity project with specified version |  [ ]  |
| Create minimal project          | `unity create /path/to/new/project --minimal`                             | Creates bare-minimum project structure (fast)    |  [ ]  |
| Create minimal project          | `unity create /path/to/new/project 6000.0.79f1 --minimal (not installed)` | Creates bare-minimum project structure (fast)    |  [ ]  |
| Create project with dry-run     | `unity create /path/to/new/project --dry-run`                             | Shows what would be executed without creating    |  [ ]  |
| Create in existing directory    | `unity create /existing/path`                                             | Shows error or warning about existing directory  |  [ ]  |
| Create with uninstalled version | `unity create /path/to/new/project 9999.9.9f1`                            | Shows error about version not being installed    |  [ ]  |
| Create with invalid path        | `unity create @$%^@#`                                                     | Shows error about invalid path                   |  [ ]  |

### `install`

| Test Case                      | Input                                           | Expected Outcome                                              | Pass? |
|--------------------------------|-------------------------------------------------|---------------------------------------------------------------|:-----:|
| Valid Unity version            | `unity install 2022.3.10f1`                     | Unity version is installed successfully, revision was fetched |  [ ]  |
| Valid version with revision    | `unity install 2022.3.10f1 ff3792e53c62`        | Unity version is installed successfully                       |  [ ]  |
| Non-existent Unity version     | `unity install 9999.9.9f1`                      | Shows error                                                   |  [ ]  |
| Already installed version      | `unity install 2022.3.10f1` (already installed) | Success message shown (idempotent operation)                  |  [ ]  |
| Install with Unity Hub modules | `unity install 2022.3.10f1 -- --module webgl`   | Installs with additional Hub arguments passed through         |  [ ]  |
| Install with dry-run           | `unity install 2022.3.10f1 --dry-run`           | Shows what would be executed without installing               |  [ ]  |

### `editor-revision`

| Test Case                  | Input                               | Expected Outcome                                            | Pass? |
|----------------------------|-------------------------------------|-------------------------------------------------------------|:-----:|
| Valid Unity version        | `unity editor-revision`             | Searches for available editors, prints version and revision |  [ ]  |
| Valid Unity version        | `unity editor-revision 2022.3.10f1` | Outputs the revision hash                                   |  [ ]  |
| Non-existent Unity version | `unity editor-revision 9999.9.9f1`  | Shows error                                                 |  [ ]  |

### `editor-path`

| Test Case                    | Input                                          | Expected Outcome                                | Pass? |
|------------------------------|------------------------------------------------|-------------------------------------------------|:-----:|
| Installed Unity version      | `unity editor-path`                            | Searches for available editors                  |  [ ]  |
| Installed Unity version      | `unity editor-path 2022.3.10f1`                | Outputs the full path to installed Unity editor |  [ ]  |
| Non-installed Unity version  | `unity editor-path 2023.1.1f1` (not installed) | Shows error                                     |  [ ]  |
| Invalid Unity version format | `unity editor-path invalid`                    | Shows error                                     |  [ ]  |

### `project-version`

| Test Case                         | Input                                     | Expected Outcome                                           | Pass? |
|-----------------------------------|-------------------------------------------|------------------------------------------------------------|:-----:|
| Valid Unity project directory     | `unity project-version /path/to/project`  | Outputs version (and revision if present)                  |  [ ]  |
| Current directory with project    | `unity project-version .`                 | Outputs version from current directory's project           |  [ ]  |
| Invalid directory                 | `unity project-version /nonexistent/path` | Shows error                                                |  [ ]  |
| Project with version only         | `unity project-version /path/to/project`  | Outputs version only (e.g. "2022.3.10f1")                  |  [ ]  |
| Project with version and revision | `unity project-version /path/to/project`  | Outputs version and revision (e.g. "2022.3.10f1 abcd1234") |  [ ]  |

### `project-path`

| Test Case                    | Input                                          | Expected Outcome                                      | Pass? |
|------------------------------|------------------------------------------------|-------------------------------------------------------|:-----:|
| Valid project directory      | `unity project-path /path/to/project`          | Outputs the project root path                         |  [ ]  |
| Subdirectory of project      | `unity project-path /path/to/project/Assets`   | Outputs the project root path (upward search)         |  [ ]  |
| Directory containing project | `unity project-path /path/containing/projects` | Outputs the found project root path (downward search) |  [ ]  |
| Current directory            | `unity project-path .`                         | Outputs project root path from current directory      |  [ ]  |
| Interactive selection        | `unity project-path` (with recent projects)    | Shows interactive prompt and outputs selected path    |  [ ]  |
| Interactive favorites only   | `unity project-path --favorite`                | Shows only favorite projects in selection prompt      |  [ ]  |
| Directory without project    | `unity project-path /path/without/project`     | Shows error                                           |  [ ]  |
| Invalid directory            | `unity project-path /nonexistent/path`         | Shows error                                           |  [ ]  |

### `version-usage`

| Test Case                            | Input                                  | Expected Outcome                                           | Pass? |
|--------------------------------------|----------------------------------------|------------------------------------------------------------|:-----:|
| Display version usage                | `unity version-usage`                  | Lists installed versions with indication of which are used |  [ ]  |
| Display version usage (plaintext)    | `unity version-usage --plaintext`      | Lists installed versions in machine-parseable format       |  [ ]  |
| No installed versions                | `unity version-usage` (none installed) | Shows message about no installed versions                  |  [ ]  |
| No projects using installed versions | `unity version-usage` (unused)         | Shows all versions as unused                               |  [ ]  |

### `projects-using`

| Test Case                     | Input                                       | Expected Outcome                                            | Pass? |
|-------------------------------|---------------------------------------------|-------------------------------------------------------------|:-----:|
| Valid installed version       | `unity projects-using 2022.3.10f1`          | Lists all projects using that version                       |  [ ]  |
| Interactive version selection | `unity projects-using` (no arg)             | Shows interactive version prompt, logs version and projects |  [ ]  |
| Version with no projects      | `unity projects-using 2022.3.10f1` (unused) | Shows empty result message                                  |  [ ]  |
| Non-installed version         | `unity projects-using 9999.9.9f1`           | Shows empty result message (no easy way to verify)          |  [ ]  |

### `install-missing`

| Test Case                        | Input                                   | Expected Outcome                                               | Pass? |
|----------------------------------|-----------------------------------------|----------------------------------------------------------------|:-----:|
| Install missing versions         | `unity install-missing`                 | Shows a prompt before installing                               |  [ ]  |
| Install missing versions         | `unity install-missing`                 | Installs all Unity versions used by projects but not installed |  [ ]  |
| Install missing with --yes       | `unity install-missing --yes`           | Skips confirmation prompt and installs missing versions        |  [ ]  |
| Install missing with dry-run     | `unity install-missing --dry-run`       | Shows what would be installed without installing               |  [ ]  |
| Install missing with Hub modules | `unity install-missing -- --module ios` | Installs missing versions with additional Hub arguments        |  [ ]  |
| No missing versions              | `unity install-missing` (all installed) | Shows message that no versions need to be installed            |  [ ]  |
| No projects found                | `unity install-missing` (no projects)   | Shows message about no projects requiring installation         |  [ ]  |

### `uninstall-unused`

| Test Case                     | Input                                  | Expected Outcome                                       | Pass? |
|-------------------------------|----------------------------------------|--------------------------------------------------------|:-----:|
| Uninstall unused versions     | `unity uninstall-unused`               | Shows a prompt before uninstalling                     |  [ ]  |
| Uninstall unused versions     | `unity uninstall-unused`               | Uninstalls all Unity versions not used by any projects |  [ ]  |
| Uninstall unused with --yes   | `unity uninstall-unused --yes`         | Skips confirmation prompt and uninstalls unused versions |  [ ]  |
| Uninstall unused with dry-run | `unity uninstall-unused --dry-run`     | Shows what would be uninstalled without uninstalling   |  [ ]  |
| No unused versions            | `unity uninstall-unused` (all in use)  | Shows message that no versions can be uninstalled      |  [ ]  |
| All versions unused           | `unity uninstall-unused` (none in use) | Uninstalls all installed versions                      |  [ ]  |

### `upm-git-url`

| Test Case                          | Input                                                 | Expected Outcome                                                | Pass? |
|------------------------------------|-------------------------------------------------------|-----------------------------------------------------------------|:-----:|
| Generate UPM URL for project       | `unity upm-git-url /path/to/project`                  | Outputs git URL in UPM format for the project                   |  [ ]  |
| Generate URL for current directory | `unity upm-git-url .`                                 | Outputs git URL for current directory's project                 |  [ ]  |
| Interactive selection              | `unity upm-git-url` (no arg)                          | Shows interactive prompt, then outputs URL for selected project |  [ ]  |
| Interactive favorites only         | `unity upm-git-url --favorite`                        | Shows only favorite projects in selection prompt                |  [ ]  |
| Project without git repository     | `unity upm-git-url /path/to/project` (not a git repo) | Shows error about missing git repository                        |  [ ]  |
| Directory without project          | `unity upm-git-url /path/without/packages`            | Shows error                                                     |  [ ]  |
| Directory without project          | `unity upm-git-url /path/to/project`                  | Output line is word-wrapped dynamically                         |  [ ]  |

### `hub`

| Test Case                      | Input                                         | Expected Outcome                            | Pass? |
|--------------------------------|-----------------------------------------------|---------------------------------------------|:-----:|
| Launch Unity Hub interactively | `unity hub`                                   | Launches Unity Hub application              |  [ ]  |
| Execute Hub with arguments     | `unity hub -- --headless editors --installed` | Executes Unity Hub command and shows output |  [ ]  |

### General

| Test Case           | Input                              | Expected Outcome                           | Pass? |
|---------------------|------------------------------------|--------------------------------------------|:-----:|
| No command provided | `unity`                            | Shows help/usage information               |  [ ]  |
| Help flag           | `unity --help`                     | Shows help/usage information               |  [ ]  |
| Version flag        | `unity --version`                  | Shows version                              |  [ ]  |
| Command validation  | `unity install` (missing arg)      | All commands should validate required args |  [ ]  |
| Command validation  | `unity isntall` (spelling mistake) | Command names should be validated          |  [ ]  |

- Commands use Spectre.Console for output formatting.
- All `path` arguments can also be search directories outside or inside of Unity projects.

Manually check installed editors:

```bash
'/Applications/Unity Hub.app/Contents/MacOS/Unity Hub' -- --headless editors --installed
```

## Known Issues

1. Opening a project via our tool might not update the Unity Hub recent projects list immediately (restart required).
2. Exiting the Spectre.Console SelectionPrompt via CTRL + C leaves the cursor in a hidden state.