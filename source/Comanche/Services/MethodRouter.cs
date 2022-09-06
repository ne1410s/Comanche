using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Comanche.Exceptions;
using Comanche.Models;

namespace Comanche.Services
{
    /// <inheritdoc cref="IMethodRouter"/>
    public class MethodRouter : IMethodRouter
    {
        private const string QSwitch = "/?";
        private static readonly List<string> HelpParams = new List<string>() { "--help", "-h", QSwitch };

        /// <inheritdoc/>
        public RouteResult LocateMethod(IEnumerable<string> args, Dictionary<string, MethodInfo> routes)
        {
            var argPsv = string.Join('|', args);
            var firstDash = Math.Max(argPsv.IndexOf('/'), argPsv.IndexOf('-'));
            var routePsv = (firstDash == -1 ? argPsv : argPsv[..firstDash]).Trim('|');
            var isHelp = args.Any(arg => HelpParams.Contains(arg));

            if (!routes.ContainsKey(routePsv))
            {
                var closestPsv = GetClosestRoute(routePsv, routes.Keys);
                var options = GetOptions(closestPsv, routes.Keys);
                var prefix = closestPsv?.Replace("|", " ") ?? "";
                var fqOptions = options.Select(o => prefix.EndsWith(o) ? prefix : $"{prefix} {o}".Trim()).ToHashSet();
                var fqOpt1Psv = fqOptions.Select(o => o.Replace(" ", "|")).FirstOrDefault();
                var methodFound = fqOpt1Psv != null && routes.ContainsKey(fqOpt1Psv) && closestPsv?.StartsWith(routePsv) == true;
                if (isHelp && (fqOpt1Psv == null || methodFound || routePsv?.Length == 0 || routePsv == QSwitch))
                {
                    return new ModuleHelp(fqOptions);
                }
                else
                {
                    throw new RouteException(routePsv, closestPsv, fqOptions);
                }
            }

            var method = routes[routePsv];
            if (isHelp)
            {
                return new MethodHelp(method);
            }

            var paramArgString = argPsv.Replace(routePsv, "").Replace("|", " ");
            var paramPairs = Regex.Split(paramArgString, "\\s+[-/]+").Where(p => !string.IsNullOrWhiteSpace(p));
            var paramMap = paramPairs.Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries));
            var dupes = paramMap.GroupBy(m => m[0]).Where(c => c.Count() > 1);
            if (dupes.Any())
            {
                throw new ParamsException(dupes.Select(kvp => $"'{kvp.Key}' appears more than once."));
            }

            var dicto = paramMap.ToDictionary(kvp => kvp[0], kvp => kvp.Length == 1 ? "" : string.Join(' ', kvp.Skip(1)));
            return new MethodRoute(method, dicto);
        }

        /// <inheritdoc/>
        public string? GetClosestRoute(string? routePsv, IEnumerable<string> routeKeys)
        {
            if (routePsv == null || routeKeys.Any(r => r == routePsv || r.StartsWith(routePsv + '|')))
            {
                return routePsv;
            }

            var pipes = routePsv.Count(c => c == '|');
            for (var i = 0; i < pipes; i++)
            {
                routePsv = routePsv[..routePsv.LastIndexOf('|')];
                var rootMatch = routeKeys.FirstOrDefault(r => r.StartsWith(routePsv));
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
            routePsv = GetClosestRoute(routePsv, routeKeys) ?? "";

            var siblings = routeKeys
                .Where(r => r != routePsv && r.StartsWith(routePsv))
                .Select(r => r[routePsv.Length..].TrimStart('|').Split('|')[0]).ToList();

            if (siblings.Count == 0 && routeKeys.Contains(routePsv))
            {
                var last = routePsv.Split('|')[^1];
                siblings.Add(last);
            }

            return siblings.ToHashSet();
        }
    }
}