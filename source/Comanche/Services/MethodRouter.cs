// <copyright file="MethodRouter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Comanche.Exceptions;
    using Comanche.Models;

    /// <inheritdoc cref="IMethodRouter"/>
    public class MethodRouter : IMethodRouter
    {
        private const string QSwitch = "/?";
        private static readonly List<string> HelpParams = new() { "--help", "-h", QSwitch };

        /// <inheritdoc/>
        public RouteResult LocateMethod(
            IEnumerable<string> args,
            Dictionary<string, MethodInfo> routes)
        {
            var isHelp = args.Any(arg => HelpParams.Contains(arg));
            var argPsv = string.Join('|', args);
            var firstDash = Math.Max(
                isHelp ? argPsv.IndexOf("/?", StringComparison.OrdinalIgnoreCase) : -1,
                argPsv.IndexOf('-', StringComparison.OrdinalIgnoreCase));
            var routePsv = (firstDash == -1 ? argPsv : argPsv[..firstDash]).Trim('|');

            if (!routes.ContainsKey(routePsv))
            {
                string? closestPsv = this.GetClosestRoute(routePsv, routes.Keys);
                var options = this.GetOptions(closestPsv, routes.Keys);
                var prefix = closestPsv?
                    .Replace("|", " ", StringComparison.OrdinalIgnoreCase) ?? string.Empty;
                var fqOptions = options
                    .Select(o => prefix.EndsWith(o, StringComparison.OrdinalIgnoreCase)
                        ? prefix
                        : $"{prefix} {o}".Trim()).ToHashSet();
                string? fqOpt1Psv = fqOptions
                    .Select(o => o.Replace(" ", "|", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                bool methodFound = fqOpt1Psv != null
                    && routes.ContainsKey(fqOpt1Psv)
                    && closestPsv?.StartsWith(routePsv, StringComparison.OrdinalIgnoreCase) == true;
                if (isHelp && (fqOpt1Psv == null || methodFound || routePsv?.Length == 0 || routePsv == QSwitch))
                {
                    return new ModuleHelp(fqOptions);
                }
                else
                {
                    throw new RouteException(routePsv, closestPsv, fqOptions);
                }
            }

            MethodInfo method = routes[routePsv];
            if (isHelp)
            {
                return new MethodHelp(method);
            }

            var paramArgString = argPsv
                .Replace(routePsv, string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("|", " ", StringComparison.OrdinalIgnoreCase);
            var paramPairs = Regex.Split(paramArgString, @"\s+-+|\s+\/\?\b").Where(p => !string.IsNullOrWhiteSpace(p));
            var paramMap = paramPairs.Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries));
            var dupes = paramMap.GroupBy(m => m[0]).Where(c => c.Count() > 1);
            if (dupes.Any())
            {
                throw new ParamsException(dupes.Select(kvp => $"'{kvp.Key}' appears more than once."));
            }

            var dicto = paramMap.ToDictionary(
                kvp => kvp[0],
                kvp => kvp.Length == 1 ? string.Empty : string.Join(' ', kvp.Skip(1)));
            return new MethodRoute(method, dicto);
        }

        /// <inheritdoc/>
        public string? GetClosestRoute(string? routePsv, IEnumerable<string> routeKeys)
        {
            if (routePsv == null
                || routeKeys.Any(r => r == routePsv
                || r.StartsWith(routePsv + '|', StringComparison.OrdinalIgnoreCase)))
            {
                return routePsv;
            }

            int pipes = routePsv.Count(c => c == '|');
            for (int i = 0; i < pipes; i++)
            {
                routePsv = routePsv[..routePsv.LastIndexOf('|')];
                string rootMatch = routeKeys.FirstOrDefault(r => r.StartsWith(
                    routePsv,
                    StringComparison.OrdinalIgnoreCase));
                if (rootMatch != null)
                {
                    return rootMatch[..routePsv.Length];
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public HashSet<string> GetOptions(string? routePsv, IEnumerable<string> routeKeys)
        {
            routePsv = this.GetClosestRoute(routePsv, routeKeys) ?? string.Empty;

            var siblings = routeKeys
                .Where(r => r != routePsv && r.StartsWith(routePsv, StringComparison.OrdinalIgnoreCase))
                .Select(r => r[routePsv.Length..].TrimStart('|').Split('|')[0]).ToList();

            if (siblings.Count == 0 && routeKeys.Contains(routePsv))
            {
                string last = routePsv.Split('|')[^1];
                siblings.Add(last);
            }

            return siblings.ToHashSet();
        }
    }
}