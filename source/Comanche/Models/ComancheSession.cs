// <copyright file="ComancheSession.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using Comanche.Exceptions;
using Comanche.Extensions;
using Comanche.Services;

/// <summary>
/// A modelled session.
/// </summary>
internal class ComancheSession
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComancheSession"/> class.
    /// </summary>
    /// <param name="modules">The top-level modules.</param>
    /// <param name="cliName">The CLI name.</param>
    /// <param name="cliVersion">The CLI version.</param>
    /// <param name="comancheVersion">The Comanche version.</param>
    public ComancheSession(
        Dictionary<string, ComancheModule> modules,
        string cliName,
        string cliVersion,
        string comancheVersion)
    {
        this.Modules = modules;
        this.CliName = cliName;
        this.CliVersion = cliVersion;
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
                writer.WriteLine($"{this.CliName} v{this.CliVersion}");
                writer.WriteLine($"- CLI-ified by Comanche v{this.ComancheVersion} © ne1410s {DateTime.Today.Year}");
                return null;
            }

            var method = this.MatchMethod(route);
            if (route.IsHelp)
            {
                writer.WriteLine($"- Method: {method.Name}");
                if (method.Summary != null)
                {
                    writer.WriteLine($"- Summary: {method.Summary}");
                }

                if (method.Parameters.Count > 0)
                {
                    writer.WriteLine("- Parameters:");
                    foreach (var param in method.Parameters.Where(p => !p.Hidden))
                    {
                        var alias = param.Alias != null ? $" (-{param.Alias})" : string.Empty;
                        var summary = param.Summary != null ? $" - {param.Summary}" : string.Empty;
                        var defVal = param.GetPrintableDefault();
                        var printDefault = defVal == null ? string.Empty : $" = {defVal}";
                        var printName = param.ParameterType.ToPrintableName();

                        writer.WriteLine($"  --{param.Name}{alias} [{printName}{printDefault}]{summary}");
                    }
                }

                var returns = method.Returns != null ? $" {method.Returns}" : string.Empty;
                writer.WriteLine($"- Returns: [{method.ReturnType.ToPrintableName()}]{returns}");

                return null;
            }
            else
            {
                var parameters = method.Parameters.ParseMap(route.ParamMap, writer);
                var result = method.Call(parameters);
                writer.WriteLine($"{result}");
                return result;
            }
        }
        catch (RouteBuilderException routeEx)
        {
            var invalidRoute = route?.RouteTerms.Count != routeEx.DeepestValidTerms.Count;
            if (route?.IsHelp != true && invalidRoute)
            {
                writer.WriteLine(routeEx.Message, WriteStyle.Error);
            }

            this.MatchModule(routeEx.DeepestValidTerms, out var modules, out var methods);
            foreach (var kvp in modules)
            {
                var summary = kvp.Value.Summary != null ? $" ({kvp.Value.Summary})" : string.Empty;
                writer.WriteLine($"MODULE: {kvp.Key}{summary}");
            }

            foreach (var kvp in methods)
            {
                var summary = kvp.Value.Summary != null ? $" ({kvp.Value.Summary})" : string.Empty;
                writer.WriteLine($"METHOD: {kvp.Key}{summary}");
            }
        }
        catch (ParamBuilderException paramEx)
        {
            writer.WriteLine("Invalid parameters", WriteStyle.Error);
            foreach (var kvp in paramEx.Errors)
            {
                writer.WriteLine($"{kvp.Key}: {kvp.Value}", WriteStyle.Error);
            }
        }
        catch (ExecutionException ex)
        {
            writer.WriteLine(ex.Message, WriteStyle.Error);
        }

        return null;
    }
}
