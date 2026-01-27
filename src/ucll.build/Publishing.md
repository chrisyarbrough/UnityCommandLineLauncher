# Publishing (for Maintainers)

## Workflow

- Update the version in the `ucll.csproj` file, e.g. `1.0.0-rc.1`.
- Ensure the Readme.md and AppConfiguration.cs help are up-to-date.
- Commit and tag the commit with a prefixed version, e.g. `v1.0.0-rc.1`

The GitHub `release.yml` workflow takes care of running the build project to produce binaries and then
signs them before uploading a new GitHub release.

It then also publishes a tool package version to NuGet.

## Local Publish

This should only be necessary if the GitHub workflow breaks down and a local hotfix release is required.

Install the `gpg` utility to let the publish process sign release artifacts with your personal key.

```shell
dotnet run
```

Artifacts are signed with the default key which you have configured (or the first secret key it finds).