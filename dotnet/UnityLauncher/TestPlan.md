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
| **install**         |  7 | Valid Unity version                      | `unity-launcher install 2022.3.10f1`                              | Unity version is installed successfully, changeset was fetched |  [ ]  |
|                     |  8 | Valid version with changeset             | `unity-launcher install 2022.3.10f1 abcd1234efgh`                 | Unity version is installed successfully                        |  [ ]  |
|                     |  9 | Invalid Unity version format             | `unity-launcher install invalid.version`                          | Error during installation (API fetch or Hub operation fails)   |  [ ]  |
|                     | 10 | Non-existent Unity version               | `unity-launcher install 9999.9.9f1`                               | Error during installation (API fetch fails)                    |  [ ]  |
|                     | 11 | Already installed version                | `unity-launcher install 2022.3.10f1` (already installed)          | Success message shown (idempotent operation)                   |  [ ]  |
| **editor-revision** | 12 | Valid Unity version                      | `unity-launcher editor-revision 2022.3.10f1`                      | Outputs the changeset/revision hash                            |  [ ]  |
|                     | 13 | Invalid Unity version format             | `unity-launcher editor-revision invalid.version`                  | Error fetching revision from API (exception caught)            |  [ ]  |
|                     | 14 | Non-existent Unity version               | `unity-launcher editor-revision 9999.9.9f1`                       | Error fetching revision from API (exception caught)            |  [ ]  |
| **editor-path**     | 15 | Installed Unity version                  | `unity-launcher editor-path 2022.3.10f1`                          | Outputs the full path to installed Unity editor                |  [ ]  |
|                     | 16 | Non-installed Unity version              | `unity-launcher editor-path 2023.1.1f1` (not installed)           | Shows error                                                    |  [ ]  |
|                     | 17 | Invalid Unity version format             | `unity-launcher editor-path invalid`                              | Shows error                                                    |  [ ]  |
| **project-version** | 18 | Valid Unity project directory            | `unity-launcher project-version /path/to/project`                 | Outputs version (and changeset if present)                     |  [ ]  |
|                     | 19 | Current directory with project           | `unity-launcher project-version .`                                | Outputs version from current directory's project               |  [ ]  |
|                     | 20 | Directory without ProjectVersion.txt     | `unity-launcher project-version /path/without/version`            | Shows error                                                    |  [ ]  |
|                     | 21 | Invalid directory                        | `unity-launcher project-version /nonexistent/path`                | Shows error                                                    |  [ ]  |
|                     | 22 | Project with version only                | `unity-launcher project-version /path/to/project`                 | Outputs version only (e.g. "2022.3.10f1")                      |  [ ]  |
|                     | 23 | Project with version and changeset       | `unity-launcher project-version /path/to/project`                 | Outputs version and changeset (e.g. "2022.3.10f1 abcd1234")    |  [ ]  |
| **general**         | 24 | No command provided                      | `unity-launcher`                                                  | Shows help/usage information                                   |  [ ]  |
|                     | 25 | Help flag                                | `unity-launcher --help`                                           | Shows help information                                         |  [ ]  |
|                     | 26 | Version flag                             | `unity-launcher --version`                                        | Shows version                                                  |  [ ]  |

## Notes

- Commands use Spectre.Console for output formatting (colored error messages in red, success in green).
- Spectre.Console should handle basic command validation (missing or invalid arguments).
- All `projectPath` arguments can also be search directories outside or inside of Unity projects.


