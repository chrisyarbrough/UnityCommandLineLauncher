# Publishing (for Maintainers)

The GitHub `release.yml` workflow takes care of running this project to produce binaries and then
signs them before uploading a new release.

To publish locally:

Install the `gpg` utility to let the publish process sign release artifacts with your personal key.

```shell
dotnet run
```

Artifacts are signed with the default key which you have configured (or the first secret key it finds).