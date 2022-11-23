// <copyright file="ComancheSession.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comanche.Exceptions;
using Comanche.Extensions;
using Comanche.Services;

/// <summary>
/// A modelled session.
/// </summary>
public class ComancheSession
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComancheSession"/> class.
    /// </summary>
    /// <param name="modules">The top-level modules.</param>
    public ComancheSession(Dictionary<string, ComancheModule> modules)
    {
        this.Modules = modules;
    }

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
    public async Task<object?> FulfilAsync(string[] args, IOutputWriter writer)
    {
        ComancheRoute? route = null;
        try
        {
            route = args.BuildRoute();
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
                    foreach (var param in method.Parameters)
                    {
                        var alias = param.Alias != null ? $" (-{param.Alias})" : string.Empty;
                        var type = param.ParameterType;
                        var defVal = param.HasDefault && (type.IsPrimitive || type == typeof(string))
                            ? $" = {param.DefaultValue}"
                            : string.Empty;
                        writer.WriteLine($"  --{param.Name}{alias} [{type.Name}{defVal}] - {param.Summary}");
                    }
                }

                writer.WriteLine($"- Returns: [{method.ReturnType.Name}] {method.Returns}");

                return null;
            }
            else
            {
                var parameters = method.Parameters.ParseMap(route.ParamMap);
                var result = await method.CallAsync(parameters);
                writer.WriteLine($"{result}");
                return result;
            }
        }
        catch (RouteBuilderException routeEx)
        {
            if (route?.IsHelp != true)
            {
                writer.WriteLine(routeEx.Message, true);
            }

            this.MatchModule(routeEx.DeepestValidTerms, out var modules, out var methods);
            if (modules.Count > 0)
            {
                writer.WriteLine("- Modules:");
                foreach (var kvp in modules)
                {
                    var summary = kvp.Value.Summary != null ? $" ({kvp.Value.Summary})" : string.Empty;
                    writer.WriteLine($"  - {kvp.Key}{summary}");
                }
            }

            if (methods.Count > 0)
            {
                writer.WriteLine("- Methods:");
                foreach (var kvp in methods)
                {
                    var summary = kvp.Value.Summary != null ? $" ({kvp.Value.Summary})" : string.Empty;
                    writer.WriteLine($"  - {kvp.Key}{summary}");
                }
            }
        }
        catch (ParamBuilderException paramEx)
        {
            writer.WriteLine("Invalid parameters", true);
            foreach (var kvp in paramEx.Errors)
            {
                writer.WriteLine($"{kvp.Key}: {kvp.Value}", true);
            }
        }
        catch (Exception ex)
        {
            writer.WriteLine(ex.Message, true);
        }

        return null;
    }
}
