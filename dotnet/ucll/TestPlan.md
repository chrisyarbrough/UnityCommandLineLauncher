# Test Plan

This document contains all test cases that need to be verified before releasing a new version.

## Test Cases

| Command             | ID | Test Case                                | Input                                                             | Expected Outcome                                               | Pass? |
|---------------------|---:|------------------------------------------|-------------------------------------------------------------------|----------------------------------------------------------------|:-----:|
| **open**            |  1 | Valid project path                       | `unity-launcher open /path/to/project` (with installed version)   | Editor launches successfully with project                      |  [ ]  |
|                     |  2 | Valid project with current directory     | `unity-launcher open .`                                           | Editor launches successfully with current directory as project |  [ ]  |
|                     |  3 | Invalid project path                     | `unity-launcher open /path/to/nonexistent`                        | Shows error                                                    |  [ ]  |
|                     |  4 | Project with additional Unity args       | `unity-launcher open /path/to/project -batchmode -quit`           | Editor launches with additional arguments passed through       |  [ ]  |
|                     |  5 | Project missing ProjectVersion.txt       | `unity-launcher open /path/to/project/without/version`            | Error parsing ProjectVersion.txt (exception caught)            |  [ ]  |
|                     |  6 | Project with Unity version not installed | `unity-launcher open /path/to/project` (with uninstalled version) | Attempts to install version, then launches editor              |  [ ]  |
|                     |  7 | Interactive selection (recent projects)  | `unity-launcher open` (with recent projects in Unity Hub)         | Shows interactive selection prompt with recent projects        |  [ ]  |
|                     |  8 | Interactive selection with search        | `unity-launcher open` → type search term                          | Filters projects by search term (substring match)              |  [ ]  |
|                     |  9 | Interactive selection - no projects      | `unity-launcher open` (no recent projects in Unity Hub)           | Shows error: "No recent Unity projects found"                  |  [ ]  |
|                     | 10 | Interactive favorites only               | `unity-launcher open --favorites` (with favorite projects)        | Shows only favorite projects in selection prompt               |  [ ]  |
|                     | 11 | Interactive favorites - no favorites     | `unity-launcher open --favorites` (no favorites marked)           | Shows error: "No favorite Unity projects found"                |  [ ]  |
|                     | 12 | Favorites flag alias (-f)                | `unity-launcher open -f`                                          | Shows only favorite projects (same as --favorites)             |  [ ]  |
|                     | 13 | Favorites flag alias (--favorite)        | `unity-launcher open --favorite`                                  | Shows only favorite projects (same as --favorites)             |  [ ]  |
| **install**         | 14 | Valid Unity version                      | `unity-launcher install 2022.3.10f1`                              | Unity version is installed successfully, changeset was fetched |  [ ]  |
|                     | 15 | Valid version with changeset             | `unity-launcher install 2022.3.10f1 ff3792e53c62`                 | Unity version is installed successfully                        |  [ ]  |
|                     | 16 | Invalid Unity version format             | `unity-launcher install invalid.version`                          | Error during installation (API fetch or Hub operation fails)   |  [ ]  |
|                     | 17 | Non-existent Unity version               | `unity-launcher install 9999.9.9f1`                               | Error during installation (API fetch fails)                    |  [ ]  |
|                     | 18 | Already installed version                | `unity-launcher install 2022.3.10f1` (already installed)          | Success message shown (idempotent operation)                   |  [ ]  |
| **editor-revision** | 19 | Valid Unity version                      | `unity-launcher editor-revision 2022.3.10f1`                      | Outputs the changeset/revision hash                            |  [ ]  |
|                     | 20 | Invalid Unity version format             | `unity-launcher editor-revision invalid.version`                  | Error fetching revision from API (exception caught)            |  [ ]  |
|                     | 21 | Non-existent Unity version               | `unity-launcher editor-revision 9999.9.9f1`                       | Error fetching revision from API (exception caught)            |  [ ]  |
| **editor-path**     | 22 | Installed Unity version                  | `unity-launcher editor-path 2022.3.10f1`                          | Outputs the full path to installed Unity editor                |  [ ]  |
|                     | 23 | Non-installed Unity version              | `unity-launcher editor-path 2023.1.1f1` (not installed)           | Shows error                                                    |  [ ]  |
|                     | 24 | Invalid Unity version format             | `unity-launcher editor-path invalid`                              | Shows error                                                    |  [ ]  |
| **project-version** | 25 | Valid Unity project directory            | `unity-launcher project-version /path/to/project`                 | Outputs version (and changeset if present)                     |  [ ]  |
|                     | 26 | Current directory with project           | `unity-launcher project-version .`                                | Outputs version from current directory's project               |  [ ]  |
|                     | 27 | Directory without ProjectVersion.txt     | `unity-launcher project-version /path/without/version`            | Shows error                                                    |  [ ]  |
|                     | 28 | Invalid directory                        | `unity-launcher project-version /nonexistent/path`                | Shows error                                                    |  [ ]  |
|                     | 29 | Project with version only                | `unity-launcher project-version /path/to/project`                 | Outputs version only (e.g. "2022.3.10f1")                      |  [ ]  |
|                     | 30 | Project with version and changeset       | `unity-launcher project-version /path/to/project`                 | Outputs version and changeset (e.g. "2022.3.10f1 abcd1234")    |  [ ]  |
| **general**         | 31 | No command provided                      | `unity-launcher`                                                  | Shows help/usage information                                   |  [ ]  |
|                     | 32 | Help flag                                | `unity-launcher --help`                                           | Shows help information                                         |  [ ]  |
|                     | 33 | Version flag                             | `unity-launcher --version`                                        | Shows version                                                  |  [ ]  |

## Notes

- Commands use Spectre.Console for output formatting (colored error messages in red, success in green).
- Spectre.Console should handle basic command validation (missing or invalid arguments).
- All `projectPath` arguments can also be search directories outside or inside of Unity projects.
- Interactive project selection reads from Unity Hub's `projects-v1.json` config file.
- Projects are sorted by most recently opened (based on `lastModified` timestamp).
- Interactive selection supports search functionality (substring matching) via Spectre.Console's `EnableSearch()`.
- The `--favorites` flag filters projects based on Unity Hub's `isFavorite` field.

Manually check installed editors:

```bash
'/Applications/Unity Hub.app/Contents/MacOS/Unity Hub' -- --headless editors --installed
```

## Known Issues

Opening a project via the launcher might not update the Unity Hub recent projects list immediately; 
a Hub restart is required.

Exiting the Spectre.Console SelectionPrompt via CTRL + C leaves the cursor in a hidden state.