// <copyright file="ComancheRoute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System.Collections.Generic;

/// <summary>
/// A modelled route.
/// </summary>
internal class ComancheRoute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComancheRoute"/> class.
    /// </summary>
    /// <param name="routeTerms">The route terms.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="isHelp">Whether a request for help.</param>
    /// <param name="isDebug">Whether to write out full exception details.</param>
    /// <param name="isVersion">Whether a request for version information.</param>
    public ComancheRoute(
        IList<string> routeTerms,
        Dictionary<string, List<string>> parameters,
        bool isHelp,
        bool isDebug,
        bool isVersion)
    {
        this.RouteTerms = routeTerms;
        this.ParamMap = parameters;
        this.IsHelp = isHelp;
        this.IsDebug = isDebug;
        this.IsVersion = isVersion;
    }

    /// <summary>
    /// Gets the route terms.
    /// </summary>
    public IList<string> RouteTerms { get; }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    public IReadOnlyDictionary<string, List<string>> ParamMap { get; }

    /// <summary>
    /// Gets a value indicating whether help was requested.
    /// </summary>
    public bool IsHelp { get; }

    /// <summary>
    /// Gets a value indicating whether to write full exception details.
    /// </summary>
    public bool IsDebug { get; }

    /// <summary>
    /// Gets a value indicating whether version information was requested.
    /// </summary>
    public bool IsVersion { get; }
}
