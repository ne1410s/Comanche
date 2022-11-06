// <copyright file="ComancheSession.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using Comanche.Exceptions;
using Comanche.Extensions;

/// <summary>
/// A modelled session.
/// </summary>
public class ComancheSession
{
    /// <summary>
    /// Initialises a new instance of the <see cref="ComancheSession"/> class.
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
    /// <returns>The result.</returns>
    public object? Fulfil(string[]? args = null)
    {
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();

        try
        {
            var route = args.BuildRoute();
            var method = this.Match(route);
            return method.Execute(route.Parameters);
        }
        catch (RouteBuilderException buildEx)
        {
            // TODO: Print help text
            return null;
        }
    }
}
