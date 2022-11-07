// <copyright file="MatchingExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using System.Collections.Generic;
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
        var matchedTerms = new List<string>();
        if (route.RouteTerms.Count == 0 || !session.Modules.ContainsKey(route.RouteTerms[0]))
        {
            throw new RouteBuilderException(Array.Empty<string>());
        }

        var firstTerm = route.RouteTerms[0];
        var module = session.Modules[firstTerm];
        matchedTerms.Add(firstTerm);
        var retVal = (ComancheMethod?)null;

        for (var i = 1; i < route.RouteTerms.Count; i++)
        {
            var iterRoute = route.RouteTerms[i];
            if (!route.IsHelp && i == route.RouteTerms.Count - 1 && module.Methods.ContainsKey(iterRoute))
            {
               retVal = module.Methods[iterRoute];
            }
            else if (module.SubModules.ContainsKey(iterRoute))
            {
                module = module.SubModules[iterRoute];
                matchedTerms.Add(iterRoute);
            }
            else
            {
                break;
            }
        }

        return retVal ?? throw new RouteBuilderException(matchedTerms);
    }
}
