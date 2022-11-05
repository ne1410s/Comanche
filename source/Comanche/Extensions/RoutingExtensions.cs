// <copyright file="RoutingExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
    /// Routes arguments for a pre-loaded session.
    /// </summary>
    /// <param name="session">The Comanche session.</param>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The result.</returns>
    public static object? Route(this ComancheSession session, string[]? args = null)
    {
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
        var route = args.BuildRoute();

        //TODO: If route is ComancheRoute isHelp, or RouteBuilderEx or RouteFinderEx
        // then we should show help for the deepest valid terms

        throw new NotImplementedException();
    }

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
        var parameters = numberedArgs.Where(kvp => Regex.IsMatch(kvp.arg, "^[-/]")).ToList();

        // Kick out no routes or have anything pre-route
        if (routeTerms.FirstOrDefault()?.index != 0)
        {
            throw new RouteBuilderException(new());
        }

        // Kick out if any args neither route nor parameter
        var termsList = routeTerms.Select(kvp => kvp.arg).ToList();
        var paramList = parameters.ConvertAll(kvp => kvp.arg);
        var firstFail = numberedArgs.FirstOrDefault(kvp =>
            !termsList.Contains(kvp.arg) && !paramList.Contains(kvp.arg));
        if (firstFail != null)
        {
            var preTerms = routeTerms.Where(r => r.index < firstFail.index);
            throw new RouteBuilderException(preTerms.Select(r => r.arg).ToList());
        }

        // Kick out if any parameter precedes a routes
        var maxTermAt = routeTerms.Last().index;
        if (parameters.Count != 0 && parameters[0].index < maxTermAt)
        {
            var preTerms = routeTerms.Where(r => r.index < parameters[0].index);
            throw new RouteBuilderException(preTerms.Select(r => r.arg).ToList());
        }

        var nonHelpParams = parameters
            .Where(kvp => !HelpArgs.Contains(kvp.arg))
            .Select(kvp => kvp.arg)
            .ToList();

        var isHelp = nonHelpParams.Count < parameters.Count;
        return new(termsList, nonHelpParams, isHelp);
    }
}
