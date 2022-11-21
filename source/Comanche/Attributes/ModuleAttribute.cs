// <copyright file="ModuleAttribute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Attributes
{
    using System;

    /// <summary>
    /// Configures a class as an execution module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : AliasAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleAttribute"/> class.
        /// </summary>
        /// <param name="name">The alias name.</param>
        public ModuleAttribute(string name)
            : base(name)
        { }
    }
}
