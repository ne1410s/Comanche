// <copyright file="AliasAttribute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Attributes
{
    using System;

    /// <summary>
    /// Provides a member with an alias in routing and execution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AliasAttribute"/> class.
        /// </summary>
        /// <param name="name">The alias name.</param>
        public AliasAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the alias name.
        /// </summary>
        public string Name { get; }
    }
}