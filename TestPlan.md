# Test Plan

This document contains test cases that need to be verified before releasing a new version.

## Test Cases

### `open`

| Test Case                                | Input                                                   | Expected Outcome                                                | Pass? |
|------------------------------------------|---------------------------------------------------------|-----------------------------------------------------------------|:-----:|
| Valid project path                       | `ucll open /path/to/project` (with installed version)   | Editor launches successfully with project                       |  [ ]  |
| Valid project path (upward search)       | `ucll open /project/Assets`                             | Editor launches successfully with project                       |  [ ]  |
| Valid project path (downward search)     | `ucll open /repo-root`                                  | Editor launches successfully with project                       |  [ ]  |
| Valid project with current directory     | `ucll open .`                                           | Editor launches successfully with current directory as project  |  [ ]  |
| Interactive selection                    | `ucll open /path/to/repos` (multiple projects)          | Shows interactive selection prompt with found projects          |  [ ]  |
| Invalid project path                     | `ucll open /path/to/nonexistent`                        | Shows error                                                     |  [ ]  |
| Project with additional Unity args       | `ucll open /path/to/project -- -batchmode -quit`        | Editor launches with additional arguments passed through        |  [ ]  |
| Project with Unity version not installed | `ucll open /path/to/project` (with uninstalled version) | Attempts to install version, then launches editor               |  [ ]  |
| Interactive selection (recent projects)  | `ucll open` (with recent projects in Unity Hub)         | Shows interactive selection prompt with recent projects         |  [ ]  |
| Interactive selection with search        | `ucll open` → type search term (with fzf installed)     | Filters projects by search term with fuzzy matching             |  [ ]  |
| Interactive selection with search        | `ucll open` → type search term (fallback)               | Filters projects by search term with substring matching         |  [ ]  |
| Interactive selection - no projects      | `ucll open` (no recent projects in Unity Hub)           | Shows error                                                     |  [ ]  |
| Interactive favorites only               | `ucll open --favorite` (with favorite projects)         | Shows only favorite projects in selection prompt (still sorted) |  [ ]  |
| Interactive favorites - no favorites     | `ucll open --favorite` (no favorites marked)            | Shows error                                                     |  [ ]  |
| Favorites flag alias (--favorite)        | `ucll open --favorites`                                 | Alias for --favorite                                            |  [ ]  |
| Open with code editor                    | `ucll open path --code-editor`                          | Opens project's solution file in default code editor            |  [ ]  |
| Open with code editor (no solution)      | `ucll open path --code-editor` (not yet generated)      | Opens project's solution file as soon as it is generated        |  [ ]  |
| Open only code editor                    | `ucll open path --only-code-editor`                     | Opens only the code editor without launching Unity              |  [ ]  |
| Code editor flags validation             | `ucll open path --code-editor --only-code-editor`       | Shows error about mutually exclusive flags                      |  [ ]  |
| Open with dry-run                        | `ucll open path --dry-run`                              | Shows what would be executed without opening                    |  [ ]  |

### `create`

| Test Case                       | Input                                                                    | Expected Outcome                                 | Pass? |
|---------------------------------|--------------------------------------------------------------------------|--------------------------------------------------|:-----:|
| Create project with default     | `ucll create /path/to/new/project`                                       | Prompts for version and creates                  |  [ ]  |
| Create project with version     | `ucll create /path/to/new/project 2022.3.10f1`                           | Creates new Unity project with specified version |  [ ]  |
| Create minimal project          | `ucll create /path/to/new/project --minimal`                             | Creates bare-minimum project structure (fast)    |  [ ]  |
| Create minimal project          | `ucll create /path/to/new/project 6000.0.79f1 --minimal (not installed)` | Creates bare-minimum project structure (fast)    |  [ ]  |
| Create project with dry-run     | `ucll create /path/to/new/project --dry-run`                             | Shows what would be executed without creating    |  [ ]  |
| Create in existing directory    | `ucll create /existing/path`                                             | Shows error or warning about existing directory  |  [ ]  |
| Create with uninstalled version | `ucll create /path/to/new/project 9999.9.9f1`                            | Shows error about version not being installed    |  [ ]  |
| Create with invalid path        | `ucll create @$%^@#`                                                     | Shows error about invalid path                   |  [ ]  |

### `install`

| Test Case                      | Input                                          | Expected Outcome                                              | Pass? |
|--------------------------------|------------------------------------------------|---------------------------------------------------------------|:-----:|
| Valid Unity version            | `ucll install 2022.3.10f1`                     | Unity version is installed successfully, revision was fetched |  [ ]  |
| Valid version with revision    | `ucll install 2022.3.10f1 ff3792e53c62`        | Unity version is installed successfully                       |  [ ]  |
| Non-existent Unity version     | `ucll install 9999.9.9f1`                      | Shows error                                                   |  [ ]  |
| Already installed version      | `ucll install 2022.3.10f1` (already installed) | Success message shown (idempotent operation)                  |  [ ]  |
| Install with Unity Hub modules | `ucll install 2022.3.10f1 -- --module webgl`   | Installs with additional Hub arguments passed through         |  [ ]  |
| Install with dry-run           | `ucll install 2022.3.10f1 --dry-run`           | Shows what would be executed without installing               |  [ ]  |

### `editor-revision`

| Test Case                  | Input                              | Expected Outcome                                            | Pass? |
|----------------------------|------------------------------------|-------------------------------------------------------------|:-----:|
| Valid Unity version        | `ucll editor-revision`             | Searches for available editors, prints version and revision |  [ ]  |
| Valid Unity version        | `ucll editor-revision 2022.3.10f1` | Outputs the revision hash                                   |  [ ]  |
| Non-existent Unity version | `ucll editor-revision 9999.9.9f1`  | Shows error                                                 |  [ ]  |

### `editor-path`

| Test Case                    | Input                                         | Expected Outcome                                | Pass? |
|------------------------------|-----------------------------------------------|-------------------------------------------------|:-----:|
| Installed Unity version      | `ucll editor-path`                            | Searches for available editors                  |  [ ]  |
| Installed Unity version      | `ucll editor-path 2022.3.10f1`                | Outputs the full path to installed Unity editor |  [ ]  |
| Non-installed Unity version  | `ucll editor-path 2023.1.1f1` (not installed) | Shows error                                     |  [ ]  |
| Invalid Unity version format | `ucll editor-path invalid`                    | Shows error                                     |  [ ]  |

### `editor-modules`

| Test Case                    | Input                                        | Expected Outcome                                      | Pass? |
|------------------------------|----------------------------------------------|-------------------------------------------------------|:-----:|
| Installed Unity version      | `ucll editor-modules`                        | Shows interactive version prompt, lists modules       |  [ ]  |
| Installed Unity version      | `ucll editor-modules 2022.3.10f1`            | Lists all installed modules for the specified version |  [ ]  |
| Version with no modules      | `ucll editor-modules 2022.3.10f1` (no mods)  | Shows error about no modules found                    |  [ ]  |
| Non-installed Unity version  | `ucll editor-modules 2023.1.1f1` (not inst.) | Shows error                                           |  [ ]  |
| Invalid Unity version format | `ucll editor-modules invalid`                | Shows error                                           |  [ ]  |

### `project-version`

| Test Case                         | Input                                    | Expected Outcome                                           | Pass? |
|-----------------------------------|------------------------------------------|------------------------------------------------------------|:-----:|
| Valid Unity project directory     | `ucll project-version /path/to/project`  | Outputs version (and revision if present)                  |  [ ]  |
| Current directory with project    | `ucll project-version .`                 | Outputs version from current directory's project           |  [ ]  |
| Invalid directory                 | `ucll project-version /nonexistent/path` | Shows error                                                |  [ ]  |
| Project with version only         | `ucll project-version /path/to/project`  | Outputs version only (e.g. "2022.3.10f1")                  |  [ ]  |
| Project with version and revision | `ucll project-version /path/to/project`  | Outputs version and revision (e.g. "2022.3.10f1 abcd1234") |  [ ]  |

### `project-path`

| Test Case                    | Input                                         | Expected Outcome                                      | Pass? |
|------------------------------|-----------------------------------------------|-------------------------------------------------------|:-----:|
| Valid project directory      | `ucll project-path /path/to/project`          | Outputs the project root path                         |  [ ]  |
| Subdirectory of project      | `ucll project-path /path/to/project/Assets`   | Outputs the project root path (upward search)         |  [ ]  |
| Directory containing project | `ucll project-path /path/containing/projects` | Outputs the found project root path (downward search) |  [ ]  |
| Current directory            | `ucll project-path .`                         | Outputs project root path from current directory      |  [ ]  |
| Interactive selection        | `ucll project-path` (with recent projects)    | Shows interactive prompt and outputs selected path    |  [ ]  |
| Interactive favorites only   | `ucll project-path --favorite`                | Shows only favorite projects in selection prompt      |  [ ]  |
| Directory without project    | `ucll project-path /path/without/project`     | Shows error                                           |  [ ]  |
| Invalid directory            | `ucll project-path /nonexistent/path`         | Shows error                                           |  [ ]  |

### `reset-project`

| Test Case                       | Input                                           | Expected Outcome                                                    | Pass? |
|---------------------------------|-------------------------------------------------|---------------------------------------------------------------------|:-----:|
| Reset project with confirmation | `ucll reset-project /path/to/project`           | Shows confirmation prompt, then deletes cache files and directories |  [ ]  |
| Reset project from current dir  | `ucll reset-project .`                          | Deletes cache files and directories from current project            |  [ ]  |
| Reset project with --yes        | `ucll reset-project /path/to/project --yes`     | Skips confirmation and deletes cache files/directories              |  [ ]  |
| Reset project with dry-run      | `ucll reset-project /path/to/project --dry-run` | Shows what would be deleted without deleting                        |  [ ]  |
| Reset with keep user settings   | `ucll reset-project /path --keep-user-settings` | Deletes cache files but preserves UserSettings folder               |  [ ]  |
| Reset interactive selection     | `ucll reset-project` (with recent projects)     | Shows interactive prompt, then resets selected project              |  [ ]  |
| Reset interactive favorites     | `ucll reset-project --favorite`                 | Shows only favorite projects in selection prompt                    |  [ ]  |
| Cancel reset confirmation       | `ucll reset-project /path` → select "No"        | Shows cancellation message without deleting                         |  [ ]  |
| Reset deletes Library folder    | `ucll reset-project /path --yes`                | Library folder is deleted                                           |  [ ]  |
| Reset deletes .csproj files     | `ucll reset-project /path --yes`                | All .csproj files are deleted                                       |  [ ]  |
| Reset deletes .sln files        | `ucll reset-project /path --yes`                | All .sln files are deleted                                          |  [ ]  |
| Reset shows completion summary  | `ucll reset-project /path --yes`                | Shows summary of deleted directories and files                      |  [ ]  |
| Invalid directory               | `ucll reset-project /nonexistent/path`          | Shows error                                                         |  [ ]  |

### `version-usage`

| Test Case                            | Input                                 | Expected Outcome                                           | Pass? |
|--------------------------------------|---------------------------------------|------------------------------------------------------------|:-----:|
| Display version usage                | `ucll version-usage`                  | Lists installed versions with indication of which are used |  [ ]  |
| Display version usage (plaintext)    | `ucll version-usage --plaintext`      | Lists installed versions in machine-parseable format       |  [ ]  |
| No installed versions                | `ucll version-usage` (none installed) | Shows message about no installed versions                  |  [ ]  |
| No projects using installed versions | `ucll version-usage` (unused)         | Shows all versions as unused                               |  [ ]  |

### `projects-using`

| Test Case                     | Input                                      | Expected Outcome                                            | Pass? |
|-------------------------------|--------------------------------------------|-------------------------------------------------------------|:-----:|
| Valid installed version       | `ucll projects-using 2022.3.10f1`          | Lists all projects using that version                       |  [ ]  |
| Interactive version selection | `ucll projects-using` (no arg)             | Shows interactive version prompt, logs version and projects |  [ ]  |
| Version with no projects      | `ucll projects-using 2022.3.10f1` (unused) | Shows empty result message                                  |  [ ]  |
| Non-installed version         | `ucll projects-using 9999.9.9f1`           | Shows empty result message (no easy way to verify)          |  [ ]  |

### `install-missing`

| Test Case                        | Input                                  | Expected Outcome                                               | Pass? |
|----------------------------------|----------------------------------------|----------------------------------------------------------------|:-----:|
| Install missing versions         | `ucll install-missing`                 | Shows a prompt before installing                               |  [ ]  |
| Install missing versions         | `ucll install-missing`                 | Installs all Unity versions used by projects but not installed |  [ ]  |
| Install missing with --yes       | `ucll install-missing --yes`           | Skips confirmation prompt and installs missing versions        |  [ ]  |
| Install missing with dry-run     | `ucll install-missing --dry-run`       | Shows what would be installed without installing               |  [ ]  |
| Install missing with Hub modules | `ucll install-missing -- --module ios` | Installs missing versions with additional Hub arguments        |  [ ]  |
| No missing versions              | `ucll install-missing` (all installed) | Shows message that no versions need to be installed            |  [ ]  |
| No projects found                | `ucll install-missing` (no projects)   | Shows message about no projects requiring installation         |  [ ]  |

### `uninstall-unused`

| Test Case                     | Input                                 | Expected Outcome                                         | Pass? |
|-------------------------------|---------------------------------------|----------------------------------------------------------|:-----:|
| Uninstall unused versions     | `ucll uninstall-unused`               | Shows a prompt before uninstalling                       |  [ ]  |
| Uninstall unused versions     | `ucll uninstall-unused`               | Uninstalls all Unity versions not used by any projects   |  [ ]  |
| Uninstall unused with --yes   | `ucll uninstall-unused --yes`         | Skips confirmation prompt and uninstalls unused versions |  [ ]  |
| Uninstall unused with dry-run | `ucll uninstall-unused --dry-run`     | Shows what would be uninstalled without uninstalling     |  [ ]  |
| No unused versions            | `ucll uninstall-unused` (all in use)  | Shows message that no versions can be uninstalled        |  [ ]  |
| All versions unused           | `ucll uninstall-unused` (none in use) | Uninstalls all installed versions                        |  [ ]  |

### `upm-git-url`

| Test Case                          | Input                                                | Expected Outcome                                                | Pass? |
|------------------------------------|------------------------------------------------------|-----------------------------------------------------------------|:-----:|
| Generate UPM URL for project       | `ucll upm-git-url /path/to/project`                  | Outputs git URL in UPM format for the project                   |  [ ]  |
| Generate URL for current directory | `ucll upm-git-url .`                                 | Outputs git URL for current directory's project                 |  [ ]  |
| Interactive selection              | `ucll upm-git-url` (no arg)                          | Shows interactive prompt, then outputs URL for selected project |  [ ]  |
| Interactive favorites only         | `ucll upm-git-url --favorite`                        | Shows only favorite projects in selection prompt                |  [ ]  |
| Project without git repository     | `ucll upm-git-url /path/to/project` (not a git repo) | Shows error about missing git repository                        |  [ ]  |
| Directory without project          | `ucll upm-git-url /path/without/packages`            | Shows error                                                     |  [ ]  |
| Directory without project          | `ucll upm-git-url /path/to/project`                  | Output line is word-wrapped dynamically                         |  [ ]  |

### `hub`

| Test Case                      | Input                                        | Expected Outcome                            | Pass? |
|--------------------------------|----------------------------------------------|---------------------------------------------|:-----:|
| Launch Unity Hub interactively | `ucll hub`                                   | Launches Unity Hub application              |  [ ]  |
| Execute Hub with arguments     | `ucll hub -- --headless editors --installed` | Executes Unity Hub command and shows output |  [ ]  |

### `completion`

| Test Case                  | Input                  | Expected Outcome                               | Pass? |
|----------------------------|------------------------|------------------------------------------------|:-----:|
| Generate ZSH completion    | `ucll completion`      | Outputs ZSH completion script to stdout        |  [ ]  |
| Completion script is valid | Install and test       | Tab completion works in ZSH after installation |  [ ]  |
| Unsupported shell          | `ucll completion bash` | Shows error or warning about unsupported shell |  [ ]  |

### General

| Test Case           | Input                             | Expected Outcome                           | Pass? |
|---------------------|-----------------------------------|--------------------------------------------|:-----:|
| No command provided | `ucll`                            | Shows help/usage information               |  [ ]  |
| Help flag           | `ucll --help`                     | Shows help/usage information               |  [ ]  |
| Version flag        | `ucll --version`                  | Shows version                              |  [ ]  |
| Command validation  | `ucll install` (missing arg)      | All commands should validate required args |  [ ]  |
| Command validation  | `ucll isntall` (spelling mistake) | Command names should be validated          |  [ ]  |

- Commands use Spectre.Console for output formatting.
- All `path` arguments can also be search directories outside or inside of Unity projects.

Manually check installed editors:

```bash
'/Applications/Unity Hub.app/Contents/MacOS/Unity Hub' -- --headless editors --installed
```

## Known Issues

1. Opening a project via our tool might not update the Unity Hub recent projects list immediately (restart required).
2. Exiting the Spectre.Console SelectionPrompt via CTRL + C leaves the cursor in a hidden state.