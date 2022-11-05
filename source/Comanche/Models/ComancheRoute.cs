// <copyright file="ComancheRoute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System.Collections.Generic;

/// <summary>
/// A modelled route.
/// </summary>
public class ComancheRoute
{
    /// <summary>
    /// Initialises a new instance of the <see cref="ComancheRoute"/> class.
    /// </summary>
    /// <param name="routeTerms">The route terms.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="isHelp">Whether a request for help.</param>
    public ComancheRoute(IList<string> routeTerms, IList<string> parameters, bool isHelp)
    {
        this.RouteTerms = routeTerms;
        this.Parameters = parameters;
        this.IsHelp = isHelp;
    }

    /// <summary>
    /// Gets the route terms.
    /// </summary>
    public IList<string> RouteTerms { get; }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    public IList<string> Parameters { get; }

    /// <summary>
    /// Gets a value indicating whether help was requested.
    /// </summary>
    public bool IsHelp { get; }
}
