// <copyright file="ModuleAttribute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Attributes;

using System;

/// <summary>
/// Configures a class as an execution module.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ModuleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleAttribute"/> class.
    /// </summary>
    /// <param name="name">The module name.</param>
    public ModuleAttribute(string name)
    {
        this.Name = name;
    }

    /// <summary>
    /// Gets the module name.
    /// </summary>
    public string Name { get; }
}
