# Comanche
## Overview
Comanche is a straight-talkin' module-discovery library for the command line.
The tool takes care of most of the work when trying to expose functions to the CLI.
The class structure gets translated to module hierarchy.

## Notes
### Commands
```powershell
# Restore tools
dotnet tool restore

# Run unit tests
gci **/TestResults/ | ri -r; dotnet test -c Release -s .runsettings; dotnet reportgenerator -targetdir:coveragereport -reports:**/coverage.cobertura.xml -reporttypes:"html;jsonsummary"; start coveragereport/index.html;

# Run mutation tests
gci **/StrykerOutput/ | ri -r; dotnet stryker -o;

# Pack and publish a pre-release to a local feed
$suffix="alpha001"; dotnet pack -c Release -o nu --version-suffix $suffix; dotnet nuget push "nu\*.*$suffix.nupkg" --source localdev; gci nu/ | ri -r; rmdir nu;

# Publishing a project that uses Comanche (e.g. single exe for Win64, excluding dotnet deps)
#dotnet publish MY_COMANCHE_PROJECT -p:PublishSingleFile=true -p:DebugType=Embedded -r win-x64 -c Release --sc false
```
