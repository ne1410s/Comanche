// <copyright file="ModuleAttribute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Attributes;

using System;

/// <summary>
/// Configures a class as an execution module.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ModuleAttribute(string? name = null) : Attribute
{
    /// <summary>
    /// Gets the module name.
    /// </summary>
    public string? Name { get; } = name;
}
