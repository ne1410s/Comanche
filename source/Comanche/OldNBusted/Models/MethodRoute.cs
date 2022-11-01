// <copyright file="MethodRoute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// A result for a method route operation.
    /// </summary>
    public class MethodRoute : RouteResult
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MethodRoute"/> class.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="actualParams">The actual parameters.</param>
        public MethodRoute(
            MethodInfo methodInfo,
            Dictionary<string, string> actualParams)
        {
            this.MethodInfo = methodInfo;
            this.ActualParams = actualParams;
        }

        /// <summary>
        /// Gets the method info.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets a map of actual parameters.
        /// </summary>
        public Dictionary<string, string> ActualParams { get; }
    }
}