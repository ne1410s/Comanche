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
internal static class RoutingExtensions
{
    /// <summary>
    /// Argument for providing extra exception details.
    /// </summary>
    internal const string DebugArg = "--debug";

    /// <summary>
    /// Arguments for requesting discovery help.
    /// </summary>
    internal static readonly List<string> HelpArgs = ["--help", "/?"];

    private const char Space = ' ';
    private const string ParamDelimiter = "|%%|";
    private const string VersionArg = "--version";

    /// <summary>
    /// Builds a route from arguments supplied.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>A route.</returns>
    /// <exception cref="RouteBuilderException">Route builder error.</exception>
    public static ComancheRoute BuildRoute(this string[] args)
    {
        var numberedArgs = args
            .Where(arg => !string.IsNullOrWhiteSpace(arg))
            .Select((arg, i) => new
            {
                arg, i, qRoute = char.IsLetter(arg[0]) || char.IsNumber(arg[0]),
                help = HelpArgs.Contains(arg),
                dbg = arg == DebugArg,
                ver = arg == VersionArg,
            })
            .ToList();

        var firstRoute = numberedArgs.Find(kvp => kvp.qRoute);
        var isVersion = numberedArgs.Exists(kvp => kvp.ver);
        var isHelp = numberedArgs.Count == 0 || numberedArgs.Exists(kvp => kvp.help);
        var isDebug = numberedArgs.Exists(kvp => kvp.dbg);

        // Divert version requests
        if (isVersion)
        {
            return new([], [], isHelp, isDebug, true);
        }

        // Kick out no routes or have anything pre-route
        if (!isHelp && firstRoute?.i != 0)
        {
            var message = firstRoute == null ? "No routes found." : $"Invalid route: {numberedArgs[0].arg}";
            throw new RouteBuilderException([], message);
        }

        var routeCount = numberedArgs.Find(kvp => !kvp.qRoute)?.i ?? numberedArgs.Count;
        var routes = numberedArgs.Take(routeCount).Select(kvp => kvp.arg).ToList();
        var parameters = numberedArgs.Skip(routeCount).Where(kvp => !kvp.help && !kvp.dbg).ToList();
        var paramMap = new Dictionary<string, List<string>>();

        if (parameters.Count > 0)
        {
            var concatParams = string.Join(Space, parameters.Select(kvp => kvp.arg));
            var piped = Regex.Replace(concatParams, @"\s+([-/]+)", ParamDelimiter + "$1");
            foreach (var param in piped.Split(ParamDelimiter))
            {
                var pArgs = param.Split(Space, StringSplitOptions.RemoveEmptyEntries);
                var paramId = pArgs[0];
                var paramValue = string.Join(Space, pArgs.Skip(1));
                if (paramMap.TryGetValue(paramId, out List<string> value))
                {
                    value.Add(paramValue);
                }
                else
                {
                    paramMap[paramId] = pArgs.Length > 1
                        ? new(new[] { paramValue })
                        : new();
                }
            }

            var badParams = paramMap
                .Where(kvp => !Regex.IsMatch(kvp.Key, "^([-]{1,2})[a-zA-Z]"))
                .Select(kvp => kvp.Key)
                .ToList();
            if (badParams.Count != 0)
            {
                // Stryker disable once Linq: There must always be at least one item
                var badArg = numberedArgs.First(n => badParams.Contains(n.arg)).arg;
                throw new RouteBuilderException(routes, $"Bad parameter: {badArg}");
            }
        }

        return new(routes, paramMap, isHelp, isDebug, isVersion);
    }
}
