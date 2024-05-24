// <copyright file="ComancheSession.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Comanche.Exceptions;
using Comanche.Extensions;

/// <summary>
/// A modelled session.
/// </summary>
/// <param name="modules">The top-level modules.</param>
/// <param name="cliName">The CLI name.</param>
/// <param name="cliVersion">The CLI version.</param>
/// <param name="cliDescription">The CLI description.</param>
/// <param name="comancheVersion">The Comanche version.</param>
internal sealed class ComancheSession(
    Dictionary<string, ComancheModule> modules,
    string cliName,
    string cliVersion,
    string? cliDescription,
    string comancheVersion)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };

    static ComancheSession()
    {
        JsonOpts.Converters.Add(new JsonStringEnumConverter());
    }

    /// <summary>
    /// Gets the CLI name.
    /// </summary>
    public string CliName { get; } = cliName;

    /// <summary>
    /// Gets the CLI version.
    /// </summary>
    public string CliVersion { get; } = cliVersion;

    /// <summary>
    /// Gets the CLI description.
    /// </summary>
    public string? CliDescription { get; } = cliDescription;

    /// <summary>
    /// Gets the Comanche version.
    /// </summary>
    public string ComancheVersion { get; } = comancheVersion;

    /// <summary>
    /// Gets the top-level modules.
    /// </summary>
    public IReadOnlyDictionary<string, ComancheModule> Modules { get; } = modules;

    /// <summary>
    /// Find, match and execute a command request.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="console">The console.</param>
    /// <param name="provider">The service provider.</param>
    /// <returns>The result.</returns>
    public object? Fulfil(string[] args, IConsole console, IServiceProvider provider)
    {
        ComancheRoute? route = null;
        try
        {
            route = args.BuildRoute();
            if (route.IsVersion)
            {
                if (route.IsHelp || route.IsDebug)
                {
                    throw new ParamBuilderException(new Dictionary<string, string>()
                    {
                        ["--version"] = "Command does not support --debug or --help",
                    });
                }

                console.Write(Environment.NewLine + "Module:", line: true);
                console.WriteStructured(this.CliName, null, $" v{this.CliVersion}", this.CliDescription.AsComment());

                console.Write(Environment.NewLine + "CLI-ified with");
                console.Write(" <3 ", colour: ConsoleColor.DarkRed);
                console.Write("by:", line: true);
                var comancheSuffix = $"(ne1410s © {DateTime.Today.Year})";
                console.WriteStructured("Comanche", null, $" v{this.ComancheVersion} ", comancheSuffix);
                console.WriteLine();

                return null;
            }

            var method = this.MatchMethod(route, out var module);
            if (route.IsHelp)
            {
                console.Write(Environment.NewLine + "Module:", line: true);
                var moduleText = " " + string.Join(" ", route.RouteTerms.Take(route.RouteTerms.Count - 1));
                console.WriteStructured(this.CliName, moduleText, null, module.Summary.AsComment());

                console.Write(Environment.NewLine + "Method:", line: true);
                var methodText = " " + string.Join(" ", route.RouteTerms);
                console.WriteStructured(this.CliName, methodText, null, method.Summary.AsComment());

                if (method.Parameters.Count > 0)
                {
                    console.Write(Environment.NewLine + "Parameters:", line: true);
                    foreach (var param in method.Parameters.Where(p => !p.Hidden))
                    {
                        var alias = param.Alias != null ? $" (-{param.Alias})" : string.Empty;
                        var paramText = $"--{param.Name}{alias} ";
                        var printName = param.ParameterType.ToPrintableName();
                        var defVal = param.GetPrintableDefault();
                        var printDefault = defVal == null ? string.Empty : $" = {defVal}";
                        var paramType = $"[{printName}{printDefault}]";
                        console.WriteStructured(null, paramText, paramType, param.Summary.AsComment());
                    }
                }

                console.Write(Environment.NewLine + "Returns:", line: true);
                var returnType = $"[{method.ReturnType.ToPrintableName()}]";
                console.WriteStructured(null, null, returnType, method.Returns.AsComment());
                console.WriteLine();

                return null;
            }
            else
            {
                var parameters = method.Parameters.ParseMap(route.ParamMap, provider);
                var result = method.Call(parameters);
                var output = result?.ToString() ?? string.Empty;
                var directWrite = result == null || result is string || result.GetType().IsValueType;
                if (!directWrite)
                {
                    try
                    {
                        output = JsonSerializer.Serialize(result, JsonOpts);
                    }
                    catch
                    {
                        // Sorry no dice
                    }
                }

                console.Write(output, line: true);
                return result;
            }
        }
        catch (RouteBuilderException routeEx)
        {
            var invalidRoute = route?.RouteTerms.Count != routeEx.DeepestValidTerms.Count;
            if (route?.IsHelp != true && invalidRoute)
            {
                console.WriteError(routeEx.Message, true);
            }

            this.MatchModule(routeEx.DeepestValidTerms, out var module, out var mods, out var methods);

            var routeText = " " + string.Join(" ", routeEx.DeepestValidTerms);
            if (module == null)
            {
                console.Write(Environment.NewLine + "Module:", line: true);
                console.WriteStructured(this.CliName, null, $" v{this.CliVersion}", this.CliDescription.AsComment());
            }
            else
            {
                console.Write(Environment.NewLine + "Module:", line: true);
                console.WriteStructured(this.CliName, routeText, null, module.Summary.AsComment());
            }

            var keyPrefix = routeEx.DeepestValidTerms.Count == 0 ? string.Empty : " ";
            if (mods.Count > 0)
            {
                console.Write(Environment.NewLine + "Sub Modules:", line: true);
                foreach (var kvp in mods.OrderBy(d => d.Key))
                {
                    var moduleSummary = kvp.Value.Summary.AsComment();
                    console.WriteStructured(this.CliName, routeText + keyPrefix + kvp.Key, null, moduleSummary);
                }
            }

            if (methods.Count > 0)
            {
                console.Write(Environment.NewLine + "Methods:", line: true);
                foreach (var kvp in methods)
                {
                    var moduleSummary = kvp.Value.Summary.AsComment();
                    console.WriteStructured(this.CliName, routeText + keyPrefix + kvp.Key, null, moduleSummary);
                }
            }

            console.WriteLine();
        }
        catch (ParamBuilderException paramEx)
        {
            console.Write(Environment.NewLine + "Invalid Parameters:", line: true);
            foreach (var kvp in paramEx.Errors)
            {
                console.WriteError($"{kvp.Key}: {kvp.Value}", true);
            }

            console.WritePrimary(Environment.NewLine + "Note:", true);
            console.Write($"Run again with {RoutingExtensions.HelpArgs[0]} for a full parameter list.", line: true);
            console.WriteLine();
        }
        catch (ExecutionException ex)
        {
            console.Write(Environment.NewLine + "Exception:", line: true);
            console.WriteSecondary($"[{ex.InnerException.GetType().Name}] ");
            console.WriteError(ex.Message, true);

            if (!route!.IsDebug)
            {
                console.WritePrimary(Environment.NewLine + "Note:", true);
                console.Write($"Run again with {RoutingExtensions.DebugArg} for more detail.", line: true);
            }
            else
            {
                console.Write(Environment.NewLine + "Stack Trace:", line: true);
                console.WritePrimary(ex.InvocationStack ?? string.Empty, true);
            }

            console.WriteLine();
        }

        return null;
    }
}
