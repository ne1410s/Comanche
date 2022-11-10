// <copyright file="ComancheMethod.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// A modelled method.
/// </summary>
public class ComancheMethod
{
    private readonly Func<object?, object?[], Task<object?>> taskCall;
    private readonly Func<object?> resolver;

    /// <summary>
    /// Initialises a new instance of the <see cref="ComancheMethod"/> class.
    /// </summary>
    /// <param name="name">The method name.</param>
    /// <param name="summary">The method summary.</param>
    /// <param name="resolver">The caller resolver.</param>
    /// <param name="taskCall">The call function.</param>
    /// <param name="parameters">The parameter definitions.</param>
    public ComancheMethod(
        string name,
        string? summary,
        Func<object?> resolver,
        Func<object?, object?[], Task<object?>> taskCall,
        List<ComancheParam> parameters)
    {
        this.resolver = resolver;
        this.taskCall = taskCall;
        this.Name = name;
        this.Summary = summary;
        this.Parameters = parameters;
    }

    /// <summary>
    /// Gets the method name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the method summary.
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    /// Gets a list of parameters.
    /// </summary>
    public IReadOnlyList<ComancheParam> Parameters { get; }

    /// <summary>
    /// Calls the function.
    /// </summary>
    /// <param name="parameters">The call parameters.</param>
    /// <returns>The result.</returns>
    public async Task<object?> CallAsync(object?[] parameters)
    {
        var caller = this.resolver();
        return await this.taskCall(caller, parameters);
    }
}