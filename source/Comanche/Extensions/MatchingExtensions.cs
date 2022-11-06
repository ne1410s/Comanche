// <copyright file="MatchingExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using Comanche.Exceptions;
using Comanche.Models;

/// <summary>
/// Extensions relating to matching requests to functionality.
/// </summary>
public static class MatchingExtensions
{
    /// <summary>
    /// Finds a method.
    /// </summary>
    /// <param name="session">The session.</param>
    /// <param name="route">The route.</param>
    /// <returns>A method.</returns>
    public static ComancheMethod Match(this ComancheSession session, ComancheRoute route)
    {
        if (route.IsHelp)
        {
            throw new RouteBuilderException(route.RouteTerms);
        }
    }
}
