using System.Collections.Generic;
using System.Reflection;
using Comanche.Models;

namespace Comanche.Services
{
    /// <summary>
    /// Routes methods.
    /// </summary>
    public interface IMethodRouter
    {
        /// <summary>
        /// Gets the closest route.
        /// </summary>
        /// <param name="routePsv">The route.</param>
        /// <param name="routeKeys">The route keys.</param>
        /// <returns>Closest route.</returns>
        public string? GetClosestRoute(string? routePsv, IEnumerable<string> routeKeys);

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <param name="routePsv">The route.</param>
        /// <param name="routeKeys">The route keys.</param>
        /// <returns>The options.</returns>
        public HashSet<string> GetOptions(string? routePsv, IEnumerable<string> routeKeys);

        /// <summary>
        /// Locates a method.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="routes">The routes.</param>
        /// <returns>A route result.</returns>
        public RouteResult LocateMethod(IEnumerable<string> args, Dictionary<string, MethodInfo> routes);
    }
}