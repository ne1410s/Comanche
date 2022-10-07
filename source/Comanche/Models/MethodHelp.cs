// <copyright file="MethodHelp.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models
{
    using System.Reflection;

    /// <summary>
    /// A result for a method help route operation.
    /// </summary>
    public class MethodHelp : HelpRoute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MethodHelp"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        public MethodHelp(MethodInfo method)
        {
            this.Method = method;
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        public MethodInfo Method { get; }
    }
}