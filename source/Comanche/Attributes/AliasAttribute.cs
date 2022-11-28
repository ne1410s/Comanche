// <copyright file="AliasAttribute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Attributes;

using System;

/// <summary>
/// Provides a member or parameter with an alternative name.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
public sealed class AliasAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AliasAttribute"/> class.
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