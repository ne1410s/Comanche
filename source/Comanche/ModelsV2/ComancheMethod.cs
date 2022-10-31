﻿// <copyright file="ComancheMethod.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.ModelsV2;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A modelled method.
/// </summary>
public class ComancheMethod
{
    private readonly Func<object?, object?[], object?> call;
    private readonly Func<object?> resolver;

    /// <summary>
    /// Initialises a new instance of the <see cref="ComancheMethod"/> class.
    /// </summary>
    /// <param name="name">The method name.</param>
    /// <param name="summary">The method summary.</param>
    /// <param name="resolver">The caller resolver.</param>
    /// <param name="invoker">The call function.</param>
    /// <param name="parameters">The parameter definitions.</param>
    public ComancheMethod(
        string name,
        string? summary,
        Func<object?> resolver,
        Func<object?, object?[], object?> invoker,
        List<ComancheParam> parameters)
    {
        this.resolver = resolver;
        this.call = invoker;
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
    public List<ComancheParam> Parameters { get; }

    /// <summary>
    /// Calls the function.
    /// </summary>
    /// <param name="callParams">The call parameters.</param>
    /// <returns>The result.</returns>
    public object? Call(Dictionary<ComancheParam, string?> callParams)
    {
        object?[] parameterVals = this.Parameters
            .Select(p => callParams.ContainsKey(p) ? p.Convert(callParams[p]) : null)
            .ToArray();

        var caller = this.resolver();
        return this.call(caller, parameterVals);
    }
}