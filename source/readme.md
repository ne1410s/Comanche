## DemoLibrary

``` powershell
# Restore tools
dotnet tool restore

# Run unit tests and show coverage report
gci **/TestResults/ | ri -r; dotnet test -s coverlet.runsettings -c Release; dotnet reportgenerator -targetdir:coveragereport -reports:**/coverage.cobertura.xml -reporttypes:"html"; start coveragereport/index.html;

# Run mutation tests and show report
dotnet stryker -o
```