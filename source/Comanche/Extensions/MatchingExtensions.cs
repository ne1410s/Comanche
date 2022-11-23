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
internal static class MatchingExtensions
{
    /// <summary>
    /// Finds a method.
    /// </summary>
    /// <param name="session">The session.</param>
    /// <param name="route">The route.</param>
    /// <returns>A method.</returns>
    /// <exception cref="RouteBuilderException">Route builder error.</exception>
    public static ComancheMethod MatchMethod(this ComancheSession session, ComancheRoute route)
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
            if (i == route.RouteTerms.Count - 1 && module.Methods.ContainsKey(iterRoute))
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

    /// <summary>
    /// Finds a method.
    /// </summary>
    /// <param name="session">The session.</param>
    /// <param name="routes">The route.</param>
    /// <param name="modules">Output modules.</param>
    /// <param name="methods">Output methods.</param>
    /// <exception cref="RouteBuilderException">Route builder error.</exception>
    public static void MatchModule(
        this ComancheSession session,
        IList<string> routes,
        out IReadOnlyDictionary<string, ComancheModule> modules,
        out IReadOnlyDictionary<string, ComancheMethod> methods)
    {
        modules = session.Modules;
        methods = new Dictionary<string, ComancheMethod>();
        if (routes.Count != 0)
        {
            var firstTerm = routes[0];
            var module = session.Modules[firstTerm];
            for (var i = 1; i < routes.Count; i++)
            {
                var iterRoute = routes[i];
                module = module.SubModules[iterRoute];
            }

            modules = module.SubModules;
            methods = module.Methods;
        }
    }
}
