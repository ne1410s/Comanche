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
using Comanche.Services;

/// <summary>
/// A modelled session.
/// </summary>
internal class ComancheSession
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="ComancheSession"/> class.
    /// </summary>
    /// <param name="modules">The top-level modules.</param>
    /// <param name="cliName">The CLI name.</param>
    /// <param name="cliVersion">The CLI version.</param>
    /// <param name="cliDescription">The CLI description.</param>
    /// <param name="comancheVersion">The Comanche version.</param>
    public ComancheSession(
        Dictionary<string, ComancheModule> modules,
        string cliName,
        string cliVersion,
        string? cliDescription,
        string comancheVersion)
    {
        this.Modules = modules;
        this.CliName = cliName;
        this.CliVersion = cliVersion;
        this.CliDescription = cliDescription;
        this.ComancheVersion = comancheVersion;
    }

    /// <summary>
    /// Gets the CLI name.
    /// </summary>
    public string CliName { get; }

    /// <summary>
    /// Gets the CLI version.
    /// </summary>
    public string CliVersion { get; }

    /// <summary>
    /// Gets the CLI description.
    /// </summary>
    public string? CliDescription { get; }

    /// <summary>
    /// Gets the Comanche version.
    /// </summary>
    public string ComancheVersion { get; }

    /// <summary>
    /// Gets the top-level modules.
    /// </summary>
    public IReadOnlyDictionary<string, ComancheModule> Modules { get; }

    /// <summary>
    /// Find, match and execute a command request.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="writer">The output writer.</param>
    /// <returns>The result.</returns>
    public object? Fulfil(string[] args, IOutputWriter writer)
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

                writer.Write(Environment.NewLine + "Module:", line: true);
                writer.WriteStructured(this.CliName, null, $" v{this.CliVersion}", this.CliDescription.AsComment());

                writer.Write(Environment.NewLine + "CLI-ifier:", line: true);
                var comancheSuffix = $"(ne1410s © {DateTime.Today.Year})";
                writer.WriteStructured("Comanche", null, $" v{this.ComancheVersion} ", comancheSuffix);
                writer.Write(line: true);

                return null;
            }

            var method = this.MatchMethod(route, out var module);
            if (route.IsHelp)
            {
                writer.Write(Environment.NewLine + "Module:", line: true);
                var moduleText = " " + string.Join(" ", route.RouteTerms.Take(route.RouteTerms.Count - 1));
                writer.WriteStructured(this.CliName, moduleText, null, module.Summary.AsComment());

                writer.Write(Environment.NewLine + "Method:", line: true);
                var methodText = " " + string.Join(" ", route.RouteTerms);
                writer.WriteStructured(this.CliName, methodText, null, method.Summary.AsComment());

                if (method.Parameters.Count > 0)
                {
                    writer.Write(Environment.NewLine + "Parameters:", line: true);
                    foreach (var param in method.Parameters.Where(p => !p.Hidden))
                    {
                        var alias = param.Alias != null ? $" (-{param.Alias})" : string.Empty;
                        var paramText = $"--{param.Name}{alias} ";
                        var printName = param.ParameterType.ToPrintableName();
                        var defVal = param.GetPrintableDefault();
                        var printDefault = defVal == null ? string.Empty : $" = {defVal}";
                        var paramType = $"[{printName}{printDefault}]";
                        writer.WriteStructured(null, paramText, paramType, param.Summary.AsComment());
                    }
                }

                writer.Write(Environment.NewLine + "Returns:", line: true);
                var returnType = $"[{method.ReturnType.ToPrintableName()}]";
                writer.WriteStructured(null, null, returnType, method.Returns.AsComment());
                writer.Write(line: true);

                return null;
            }
            else
            {
                var parameters = method.Parameters.ParseMap(route.ParamMap, writer);
                var result = method.Call(parameters);
                var output = result?.ToString();
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

                writer.Write(output, line: true);
                return result;
            }
        }
        catch (RouteBuilderException routeEx)
        {
            var invalidRoute = route?.RouteTerms.Count != routeEx.DeepestValidTerms.Count;
            if (route?.IsHelp != true && invalidRoute)
            {
                writer.Write(routeEx.Message, WriteStyle.Error, true);
            }

            this.MatchModule(routeEx.DeepestValidTerms, out var module, out var modules, out var methods);

            var routeText = " " + string.Join(" ", routeEx.DeepestValidTerms);
            if (module == null)
            {
                writer.Write(Environment.NewLine + "Module:", line: true);
                writer.WriteStructured(this.CliName, null, $" v{this.CliVersion}", this.CliDescription.AsComment());
            }
            else
            {
                writer.Write(Environment.NewLine + "Module:", line: true);
                writer.WriteStructured(this.CliName, routeText, null, module.Summary.AsComment());
            }

            var keyPrefix = routeEx.DeepestValidTerms.Count == 0 ? string.Empty : " ";
            if (modules.Count > 0)
            {
                writer.Write(Environment.NewLine + "Sub Modules:", line: true);
                foreach (var kvp in modules)
                {
                    var moduleSummary = kvp.Value.Summary.AsComment();
                    writer.WriteStructured(this.CliName, routeText + keyPrefix + kvp.Key, null, moduleSummary);
                }
            }

            if (methods.Count > 0)
            {
                writer.Write(Environment.NewLine + "Methods:", line: true);
                foreach (var kvp in methods)
                {
                    var moduleSummary = kvp.Value.Summary.AsComment();
                    writer.WriteStructured(this.CliName, routeText + keyPrefix + kvp.Key, null, moduleSummary);
                }
            }

            writer.Write(line: true);
        }
        catch (ParamBuilderException paramEx)
        {
            writer.Write(Environment.NewLine + "Invalid Parameters:", line: true);
            foreach (var kvp in paramEx.Errors)
            {
                writer.Write($"{kvp.Key}: {kvp.Value}", WriteStyle.Error, true);
            }

            writer.Write(Environment.NewLine + "Note:", WriteStyle.Highlight1, true);
            writer.Write($"Run again with {RoutingExtensions.HelpArgs[0]} for a full parameter list.", line: true);
            writer.Write(line: true);
        }
        catch (ExecutionException ex)
        {
            writer.Write(Environment.NewLine + "Exception:", line: true);
            writer.Write($"[{ex.InnerException.GetType().Name}] ", WriteStyle.Highlight2);
            writer.Write(ex.Message, WriteStyle.Error, true);

            if (!route!.IsDebug)
            {
                writer.Write(Environment.NewLine + "Note:", WriteStyle.Highlight1, true);
                writer.Write($"Run again with {RoutingExtensions.DebugArg} for more detail.", line: true);
            }
            else
            {
                writer.Write(Environment.NewLine + "Stack Trace:", line: true);
                writer.Write(ex.InvocationStack, WriteStyle.Highlight1, true);
            }

            writer.Write(line: true);
        }

        return null;
    }
}
