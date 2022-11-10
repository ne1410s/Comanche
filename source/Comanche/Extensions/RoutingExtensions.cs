// <copyright file="RoutingExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Comanche.Exceptions;
using Comanche.Models;

/// <summary>
/// Extensions relating to command routing.
/// </summary>
public static class RoutingExtensions
{
    private static readonly List<string> HelpArgs = new() { "-h", "--help", "/?" };

    /// <summary>
    /// Builds a route from arguments supplied.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>A route.</returns>
    public static ComancheRoute BuildRoute(this string[] args)
    {
        var numberedArgs = args
            .Select((arg, index) => new { arg, index })
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.arg))
            .ToList();

        var routeTerms = numberedArgs.Where(kvp => Regex.IsMatch(kvp.arg, "^[a-zA-Z]"));
        var firstParamAt = numberedArgs.Find(kvp => Regex.IsMatch(kvp.arg, "^[-/]"))?.index ?? -1;

        // Kick out no routes or have anything pre-route
        if (routeTerms.FirstOrDefault()?.index != 0)
        {
            throw new RouteBuilderException(Array.Empty<string>());
        }

        // Kick out if any parameter precedes a routes
        var maxTermAt = routeTerms.Last().index;
        if (firstParamAt != -1 && firstParamAt < maxTermAt)
        {
            var preTerms = routeTerms.Where(r => r.index < firstParamAt);
            throw new RouteBuilderException(preTerms.Select(r => r.arg).ToList());
        }

        var isHelp = numberedArgs.Any(kvp => HelpArgs.Contains(kvp.arg));
        var routeTermsList = routeTerms.Select(kvp => kvp.arg).ToList();
        var paramMap = new Dictionary<string, string>();

        if (firstParamAt != -1)
        {
            var concatParams = string.Join(' ', numberedArgs
                .Skip(firstParamAt)
                .Where(p => !HelpArgs.Contains(p.arg))
                .Select(a => a.arg));
            var piped = Regex.Replace(concatParams, @"\s+([-/]+)", "|%%|$1");
            paramMap = piped.Split("|%%|")
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(kvp => kvp[0], kvp => string.Join(" ", kvp.Skip(1)));

            // TODO: Dedupe params (probs not in ROUTING extensions tho..) - inc aliases
            // TODO: Think about array handling? eg --myIntVars 12 32 33
        }

        return new(routeTermsList, paramMap, isHelp);
    }
}
