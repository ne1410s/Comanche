// <copyright file="ComancheModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.ModelsV2;

using System;
using System.Collections.Generic;

/// <summary>
/// A modelled module.
/// </summary>
public class ComancheModule
{
    /// <summary>
    /// Initialises a new instance of the <see cref="ComancheModule"/> class.
    /// </summary>
    /// <param name="name">The module name.</param>
    /// <param name="summary">The summary description.</param>
    /// <param name="methods">The methods.</param>
    public ComancheModule(string name, string? summary, Dictionary<string, ComancheMethod> methods)
    {
        this.Name = name;
        this.Summary = summary;
        this.Methods = methods;
    }

    /// <summary>
    /// Gets the module name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the summary.
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    /// Gets a list of methods.
    /// </summary>
    public Dictionary<string, ComancheMethod> Methods { get; }
}
