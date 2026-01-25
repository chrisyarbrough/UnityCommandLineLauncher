# Publishing (for Maintainers)

## GitHub Release (Self-Contained Binary)

The GitHub `release.yml` workflow takes care of running this project to produce binaries for each platform and then
signs them before uploading a new release.

If this is not available, and we need to publish locally:
Install the `gpg` utility to let the publish process sign release artifacts with your personal key.

```shell
dotnet run
```

Artifacts are signed with the default key which you have configured (or the first secret key it finds).

## Dotnet Tool

```shell
PACK_DIR=bin/pack
BUILD_DIR=$PACK_DIR/build
dotnet pack ../ucll/ucll.csproj \
  -p:PackAsTool=true \
  -p:ToolCommandName=ucll \
  -p:PackageId=UnityCommandLineLauncher \
  -p:Authors="Chris Yarbrough" \
  -p:PackageLicenseExpression=MIT \
  -p:PackageReadmeFile=README.md \
  -p:PackageTags="Unity CLI Hub" \
  -p:RepositoryUrl=https://github.com/chrisyarbrough/UnityCommandLineLauncher \
  -p:RepositoryType=git \
  -p:OutputPath=$BUILD_DIR \
  -p:PublishDir=$BUILD_DIR \
  --output ../ucll/$PACK_DIR
```

```shell
dotnet nuget push ../ucll/bin/pack/UnityCommandLineLauncher.*.nupkg --api-key <your-key> --source https://apiint.nugettest.org/v3/index.json
```

```
https://api.nuget.org/v3/index.json
```