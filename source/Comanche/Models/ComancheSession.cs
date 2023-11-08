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
                writer.WriteLine(Environment.NewLine + "Module:");
                writer.Write(this.CliName, WriteStyle.Highlight1);
                writer.Write($" v{this.CliVersion}", WriteStyle.Highlight2);
                writer.WriteLine(this.CliDescription.AsComment(), WriteStyle.Highlight3);

                writer.WriteLine(Environment.NewLine + "CLI-ifier:");
                writer.Write("Comanche", WriteStyle.Highlight1);
                writer.Write($" v{this.ComancheVersion} ", WriteStyle.Highlight2);
                writer.WriteLine($"(ne1410s © {DateTime.Today.Year}){Environment.NewLine}", WriteStyle.Highlight3);

                return null;
            }

            var method = this.MatchMethod(route, out var module);
            if (route.IsHelp)
            {
                writer.WriteLine(Environment.NewLine + "Module:");
                writer.Write(this.CliName + " ", WriteStyle.Highlight1);
                writer.Write(string.Join(" ", route.RouteTerms.Take(route.RouteTerms.Count - 1)));
                writer.WriteLine(module.Summary.AsComment(), WriteStyle.Highlight3);

                writer.WriteLine(Environment.NewLine + "Method:");
                writer.Write(this.CliName + " ", WriteStyle.Highlight1);
                writer.Write(string.Join(" ", route.RouteTerms));
                writer.WriteLine(method.Summary.AsComment(), WriteStyle.Highlight3);

                if (method.Parameters.Count > 0)
                {
                    writer.WriteLine(Environment.NewLine + "Parameters:");
                    foreach (var param in method.Parameters.Where(p => !p.Hidden))
                    {
                        var alias = param.Alias != null ? $" (-{param.Alias})" : string.Empty;
                        writer.Write($"--{param.Name}{alias} ");

                        var printName = param.ParameterType.ToPrintableName();
                        var defVal = param.GetPrintableDefault();
                        var printDefault = defVal == null ? string.Empty : $" = {defVal}";
                        writer.Write($"[{printName}{printDefault}]", WriteStyle.Highlight2);
                        writer.WriteLine(param.Summary.AsComment(), WriteStyle.Highlight3);
                    }
                }

                writer.WriteLine(Environment.NewLine + "Returns:");
                writer.Write($"[{method.ReturnType.ToPrintableName()}]", WriteStyle.Highlight2);
                writer.WriteLine(method.Returns.AsComment() + Environment.NewLine, WriteStyle.Highlight3);

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

            this.MatchModule(routeEx.DeepestValidTerms, out var module, out var modules, out var methods);

            if (module == null)
            {
                writer.WriteLine(Environment.NewLine + "Module:");
                writer.Write(this.CliName, WriteStyle.Highlight1);
                writer.Write($" v{this.CliVersion}", WriteStyle.Highlight2);
                writer.WriteLine(this.CliDescription.AsComment(), WriteStyle.Highlight3);
            }
            else
            {
                writer.WriteLine(Environment.NewLine + "Module:");
                writer.Write(this.CliName + " ", WriteStyle.Highlight1);
                writer.Write(string.Join(" ", routeEx.DeepestValidTerms));
                writer.WriteLine(module.Summary.AsComment(), WriteStyle.Highlight3);
            }

            var keyPrefix = routeEx.DeepestValidTerms.Count == 0 ? string.Empty : " ";
            if (modules.Count > 0)
            {
                writer.WriteLine(Environment.NewLine + "Sub Modules:");
                foreach (var kvp in modules)
                {
                    writer.Write(this.CliName + " ", WriteStyle.Highlight1);
                    writer.Write(string.Join(" ", routeEx.DeepestValidTerms));
                    writer.Write(keyPrefix + kvp.Key);
                    writer.WriteLine(kvp.Value.Summary.AsComment(), WriteStyle.Highlight3);
                }
            }

            if (methods.Count > 0)
            {
                writer.WriteLine(Environment.NewLine + "Methods:");
                foreach (var kvp in methods)
                {
                    writer.Write(this.CliName + " ", WriteStyle.Highlight1);
                    writer.Write(string.Join(" ", routeEx.DeepestValidTerms));
                    writer.Write(keyPrefix + kvp.Key);
                    writer.WriteLine(kvp.Value.Summary.AsComment(), WriteStyle.Highlight3);
                }
            }

            writer.Write(Environment.NewLine);
        }
        catch (ParamBuilderException paramEx)
        {
            writer.WriteLine(Environment.NewLine + "Invalid Parameters:");
            foreach (var kvp in paramEx.Errors)
            {
                writer.Write($"{kvp.Key}: ", WriteStyle.Error);
                writer.WriteLine(kvp.Value, WriteStyle.Error);
                writer.WriteLine(Environment.NewLine + "Note:", WriteStyle.Highlight1);
                writer.WriteLine($"Run again with {RoutingExtensions.HelpArgs[0]} for a full parameter list.");
            }

            writer.Write(Environment.NewLine);
        }
        catch (ExecutionException ex)
        {
            writer.WriteLine(Environment.NewLine + "Exception:");

            if (!route!.IsDebug)
            {
                writer.WriteLine(ex.Message, WriteStyle.Error);
                writer.WriteLine(Environment.NewLine + "Note:", WriteStyle.Highlight1);
                writer.WriteLine($"Run again with {RoutingExtensions.DebugArg} for more detail.");
            }
            else
            {
                writer.Write($"[{ex.InnerException.GetType().Name}] ", WriteStyle.Highlight2);
                writer.WriteLine(ex.Message, WriteStyle.Error);
                writer.WriteLine(Environment.NewLine + "Stack Trace:");
                writer.WriteLine(ex.InvocationStack ?? ex.StackTrace, WriteStyle.Highlight1);
            }

            writer.Write(Environment.NewLine);
        }

        return null;
    }
}
