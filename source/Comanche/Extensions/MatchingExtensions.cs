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

    /// <summary>
    /// Prepares a boxed array of parameters, ready to be fed into the method.
    /// </summary>
    /// <param name="method">A method.</param>
    /// <param name="paramMap">A parameter map.</param>
    /// <returns>An array of parameters.</returns>
    public static object?[] GetParamMap(
        this ComancheMethod method,
        IReadOnlyDictionary<string, List<string>> paramMap)
    {
        var retVal = new List<object?>();
        var errors = new Dictionary<ComancheParam, string>();

        foreach (var param in method.Parameters)
        {
            List<string> inputs;
            var byName = paramMap.TryGetValue("--" + param.Name, out inputs);
            var byAlias = param.Alias != null && paramMap.TryGetValue("-" + param.Alias, out inputs);

            if (!byName && !byAlias)
            {
                if (!param.HasDefault)
                {
                    errors[param] = "missing";
                }
                else
                {
                    retVal.Add(param.DefaultValue);
                }
            }
            else if (byName && byAlias)
            {
                errors[param] = "duplicate";
            }
            else
            {
                // TODO: Convert list<string> to params, per-type
            }
        }

        //object?[] parameterVals = method.Parameters
        //    .Select(p => paramMap.ContainsKey(p) ? p.Convert(paramMap[p]) : null)
        //    .ToArray();

        throw new NotImplementedException();
    }
}
