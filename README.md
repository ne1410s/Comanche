# Comanche
## Overview
Comanche is a module-discovery library for the command line. 

It is designed to work for *you, the developer* by allowing you to write your code. It will sit back until CLI invocation, where is dynamically discovers and relays it (just the bits you want).

## Quick Start
Getting started is as easy as 1, 2, 3!

1. Add the latest `Comanche` package to your console app, enabling "generate documentation file" is also recommended!
2. Write some code
3. Call Comanche's [Discover.Go()](#discovergo) from your Program.cs

Now you can build and ship your exe as normal..

.. But **please keep reading!** There are many more time-saving features to explore below.

![easy as 1 2 3](docs/123.png)

## Development Guide
This section provides guidance on how to make use of all of the features and functionality of Comanche!

### Structure
All commands are made available in the following format:

> `assembly`&nbsp;&nbsp;`module`&nbsp;&nbsp;`[..sub-modules]`&nbsp;&nbsp;`method`&nbsp;&nbsp;`[..params]`

You can organise your coding entities to yield your desired CLI experience and to make for the most intuitive user experience.

In order to change the `assembly`, this can be done by setting the `<AssemblyName>` property in your csproj file. Note that this will be case-sensitive in Unix systems.

![sub module](docs/sub-module.png)

#### Modules
A Comanche Module is the direct equivalent to a C# `class`. As well as its own methods, it can also contain nested classes, which become sub-modules in Comanche.

By default, Comanche discovery attempts to expose *all* public classes in the console app to the CLI. To prevent a class from being exposed, there are several supported options:

- Decorate the classes you do *not* wish to expose with the `[Hidden]` attribute
- Set the `moduleOptIn` parameter to `true` in the call to `Discover.Go()`.  In this mode, only modules decorated with the `[Module]` attribute will be exposed to the CLI. (This also allows for an alias to be provided to the module, as per the above screenshot)
- Modify the class access to a non-public scope, such as `internal` or `private` (if appropriate)

Also by default is the module naming convention. Any "Module" suffix is removed and the remaining terms are transformed to "kebab-case". This can be overriden by providing a custom name in `[Module]` attribute.

Note that `static` classes and methods are supported by Comanche, but it should be noted that module-scoped dependency injection is not possible with a static class.

#### Methods
A Comanche Method is the direct equivalent to a C# `method`.  As with Modules, the `[Hidden]` attribute is supported for methods too. This means the method will not appear in the CLI discovery help, nor be reachable in any other way. Similar naming conventions as modules also apply for methods. To override these, the `[Alias]` parameter can be used.

Comanche fully-supports `async Task<>` methods. Comanche operates these in a way that makes the resulting user experience pretty much the same regardless of this implementation choice, with responses being handled using appropriate task resolution.

#### Parameters
A Comanche Parameter is the direct equivalent to a C# `method parameter`. In CLI terms, parameters are generally best off as primitive types or strings. This makes for optimal user experience as it is easy to handle these and to relay appropriate xml documentation to the CLI. With that said, it is possible to deliver complex types as parameters in Comanche too. To do this you must deliver the parameter as a JSON string.

Parameters should be provided with default values where appropriate, to optimise the user experience. This feeds into the CLI docs and parameter validation.

Please note there are some [reserved parameter names](#reserved-flags) which should be avoided.

#### Return Values
Invocation results are returned to the CLR from the `Discover.Go()` call. Return values are always written to stdout. In the case of strings and value types, these are written using whatever `.ToString()` implementation applies. In all other cases a JSON representation is written instead (indented camel case, omitting nulls).

### Discover.Go(...)
There are several parameters available on this top-level method. Some of which are there purely to support debugging and/or unit testing. The most useful functional parameters are described here:
|Parameter|Description|
|--|--|
|`moduleOptIn` [boolean = False]|Whether to mandate the presence of `[Module]` attribute for a class to be exposed.|
|`services` [IServiceCollection = null]|Set of injected dependencies that will be delivered to class ctors and methods.|

### Debugging
One of the parameters in `Discover.Go()` is `args`. This is useful for debugging as you can pass in the same string as per the CLI command and the run will essentially be the same, with the key difference being you have the execution thread! Your IDE should automatically provide a console window, so you can simulate the full user experience.

```csharp
string? debugCommand = null;

#if DEBUG
// To debug, uncomment the line below and edit the command string
debugCommand = "number get-random";
#endif

Discover.Go(args: debugCommand?.Split(' '));
```

*Note that the assembly is not required in the debug command string, since this is known from context.*

### Dependency Injection
As alluded to above, Comanche supports dependency injection! Simply new up a `ServicesCollection`, add your dependencies and pass it to `Discover.Go()`. You are then able to specify the registered types as class constructor parameters and method parameters alike.

Use of dependency injection helps simplify code, especially where lots of different methods share the same dependencies. If these methods are all specified in the same module (C# class) then that class can cache them as fields. This makes the code easier to read, and to unit test.

**Please note:** Dependencies injected at the *method* level must be decorated with the `[Hidden]` attribute in order for them to be resolved. This also prevents CLI consumers from having to worry (or indeed even know) about them.

### Configuration
As we're doing DI it would be rude *not* to support `IConfiguration` :D You can add an `appsettings.json` file to the top-level of your console application and (assuming you mark it as CopyAlways or CopyIfNewer) then it will be available to Comanche and parsed into IConfig. Similarly to ASP.NET web applications, Comanche also supports environment-based files (e.g. `appsettings.Development.json`) where the environment name is taken from the environment variable, `COMANCHE_ENVIRONMENT`. If no such variable is found then `Development` is used as default. Comanche also looks in your environment variables themselves to populate IConfig, which overrides anything set by json in the event of collisions.

To make use of the config mechanism, add your json file / env vars and simply define `IConfiguration config` as a parameter - at either the class ctor or method level.

### Output Writer
`[IOutputWriter]` is used internally by Comanche for writing to stdout (and stderr on occasion!) in a unit-testable way. It also has basic prompt/capture capabilities. It is injected via DI to every Comanche method call as a courtesy (but I won't be offended if you don't use it!).

## CLI Usage Guide
This section provides guidance on how to discover and use  all of the functionality within any given Comanche CLI tool!

### Basic Commands
Assuming the tool (.exe in Windows) is available within your context (i.e. in your current directory, or on your PATH), just typing the name of the assembly gets you started, by returning some basic info including the tool's published assembly version and a list of top-level modules, which it refers to as Sub Modules of the main tool.

### Route Validation
Routing is the bit that maps your command text down to a particular method. This needs to run successfully in order for Comanche to move on to parameter validation and finally execution. If a route is not understood by Comanche, a message is written to stderr in angry red text, which should provide full clarity on the issue.

### Parameter Validation
Assuming a route is found, Comanche can start checking the supplied parameters against the requirements of the method. Parameters with no default are considered as required. If these are not supplied, the command is rejected with a clear explanation. If you supply unrecognised arguments, this is similarly rejected.

### Boolean Flags
Boolean flags offer a short-hand, where you only need to provide the flag name to indicate `true`. You can pass `true` explicitly to the same effect. The value can be explicitly negated, as follows:
> `assembly module method --force false`  

### Enum Values
Enum handling is designed to be versatile between strings and numeric values. Therefore either one is acceptable for the parameter to be correctly understood. Note that in return values, the string form is used.

### Reserved Flags
There are a number of reserved flags that apply globally.

|Flag|Effect|
|--|--|
|`--debug`|In the event of an exception, this shows a stack trace.|
|`--help` `/?`|This shows information according to the route.|
|`--version`|This provides full version information.|

### Error Handling
Any exception that occurs during invocation is neatly summarised to show the exception type and message. For more information, the command may be re-run with `--debug` to yield a stack trace.

## Further Notes
### Handy Commandies
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
dotnet publish MY_COMANCHE_PROJECT -p:PublishSingleFile=true -p:DebugType=Embedded -r win-x64 -c Release --sc false
```
