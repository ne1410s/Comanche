// <copyright file="ComancheSession.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System.Collections.Generic;

/// <summary>
/// A modelled session.
/// </summary>
public class ComancheSession
{
    /// <summary>
    /// Initialises a new instance of the <see cref="ComancheSession"/> class.
    /// </summary>
    /// <param name="modules">The top-level modules.</param>
    public ComancheSession(Dictionary<string, ComancheModule> modules)
    {
        this.Modules = modules;
    }

    /// <summary>
    /// Gets the top-level modules.
    /// </summary>
    public IReadOnlyDictionary<string, ComancheModule> Modules { get; }
}
