# Running Coverage Locally

Setup:

```shell
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Generate report:

```shell
dotnet test \
	/p:CollectCoverage=true \
	/p:CoverletOutput="./TestResults/coverage.cobertura.xml" \
	/p:CoverletOutputFormat=cobertura \
	/p:ExcludeByFile="**/obj/**/*.g.cs"

reportgenerator \
	-reports:"TestResults/coverage.cobertura.xml" \
	-targetdir:"TestResults/Coverage" \
	-reporttypes:"Html"

open TestResults/Coverage/index.html

```