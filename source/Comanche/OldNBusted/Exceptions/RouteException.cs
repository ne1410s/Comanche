// <copyright file="RouteException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Exceptions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents errors occuring during the processing of routes.
    /// </summary>
    public class RouteException : ComancheException
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="RouteException"/> class.
        /// </summary>
        /// <param name="requestedRoutePsv">The requested route.</param>
        /// <param name="closestRoutePsv">The closest route.</param>
        /// <param name="closestRouteOpts">The closest route options.</param>
        public RouteException(
            string? requestedRoutePsv,
            string? closestRoutePsv,
            HashSet<string> closestRouteOpts)
                : base(GetMessage(closestRouteOpts))
        {
            this.RequestedRoute = requestedRoutePsv;
            this.ClosestRoute = closestRoutePsv;
            this.ClosestRouteOpts = closestRouteOpts;
        }

        /// <summary>
        /// Gets the requested route.
        /// </summary>
        public string? RequestedRoute { get; }

        /// <summary>
        /// Gets the closest route.
        /// </summary>
        public string? ClosestRoute { get; }

        /// <summary>
        /// Gets the closest route options.
        /// </summary>
        public HashSet<string> ClosestRouteOpts { get; }

        private static string GetMessage(HashSet<string> fqOpts)
            => $"Command not recognised. Suggestions:{Environment.NewLine}  > "
                + string.Join($"{Environment.NewLine}  > ", fqOpts);
    }
}