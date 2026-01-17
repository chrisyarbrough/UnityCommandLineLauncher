Install the `gpg` utility to let the publish process sign release artifacts.

To publish, run this project:

```shell
dotnet run
```

Artifacts are signed with the default key which you have configured (or the first secret key it finds).