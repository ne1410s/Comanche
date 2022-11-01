// <copyright file="IRouteBuilder.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services
{
    using System.Collections.Generic;
    using System.Reflection;

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