// <copyright file="AliasAttribute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche;

using System;

/// <summary>
/// Provides a module, method or parameter with an alternative name.
/// </summary>
/// <param name="name">The alias name.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
public sealed class AliasAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the alias name.
    /// </summary>
    public string Name { get; } = name;
}