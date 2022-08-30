using System.Collections.Generic;
using System.Reflection;

namespace Comanche.Services
{
    /// <summary>
    /// Builds routes.
    /// </summary>
    public interface IRouteBuilder
    {
        /// <summary>
        /// Builds routes for an assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>A route dictionary.</returns>
        public Dictionary<string, MethodInfo> BuildRoutes(Assembly assembly);
    }
}