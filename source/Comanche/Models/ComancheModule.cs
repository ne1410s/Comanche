// <copyright file="ComancheModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System.Collections.Generic;

/// <summary>
/// A modelled module.
/// </summary>
/// <param name="name">The module name.</param>
/// <param name="summary">The summary description.</param>
/// <param name="methods">The methods.</param>
/// <param name="subModules">The sub-modules.</param>
internal sealed class ComancheModule(
    string name,
    string? summary,
    Dictionary<string, ComancheMethod> methods,
    Dictionary<string, ComancheModule> subModules)
{
    /// <summary>
    /// Gets the module name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the summary.
    /// </summary>
    public string? Summary { get; } = summary;

    /// <summary>
    /// Gets a list of methods.
    /// </summary>
    public IReadOnlyDictionary<string, ComancheMethod> Methods { get; } = methods;

    /// <summary>
    /// Gets a list of sub-modules.
    /// </summary>
    public IReadOnlyDictionary<string, ComancheModule> SubModules { get; } = subModules;
}
