// <copyright file="ComancheSession.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;
using System.Collections.Generic;
using System.Linq;
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
    public async Task<object?> FulfilAsync(string[]? args = null, IOutputWriter? writer = null)
    {
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
        writer ??= new ConsoleWriter();

        try
        {
            var route = args.BuildRoute();
            var method = this.MatchMethod(route);
            var parameters = method.Parameters.ParseMap(route.ParamMap);
            var result = await method.CallAsync(parameters);
            writer.WriteLine($"{result}");
            return result;
        }
        catch (RouteBuilderException routeEx)
        {
            this.MatchModule(routeEx.DeepestValidTerms, out var modules, out var methods);
            writer.WriteLine("Possible options", true);
            foreach (var kvp in modules)
            {
                writer.WriteLine($" > {kvp.Key} < {kvp.Value.Summary}");
            }

            foreach (var kvp in methods)
            {
                writer.WriteLine($" ~ {kvp.Key}() {kvp.Value.Summary}");
            }
        }
        catch (ParamBuilderException paramEx)
        {
            writer.WriteLine("Invalid parameters", true);
            foreach (var kvp in paramEx.Errors)
            {
                writer.WriteLine($" {kvp.Key}: {kvp.Value}", true);
            }
        }
        catch (Exception ex)
        {
            writer.WriteLine(ex.Message, true);
        }

        return null;
    }
}
