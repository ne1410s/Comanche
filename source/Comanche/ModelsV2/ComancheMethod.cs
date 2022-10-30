// <copyright file="ComancheMethod.cs" company="ne1410s">
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
    private readonly Func<Type, object?[], object?> call;

    /// <summary>
    /// Initialises a new instance of the <see cref="ComancheMethod"/> class.
    /// </summary>
    /// <param name="name">The method name.</param>
    /// <param name="parent">The parent module.</param>
    /// <param name="parameters">The parameter definitions.</param>
    /// <param name="call">The call function.</param>
    public ComancheMethod(
        string name,
        ComancheModule parent,
        List<ComancheParam> parameters,
        Func<Type, object?[], object?> call)
    {
        this.Name = name;
        this.Parent = parent;
        this.Parameters = parameters;
        this.call = call;
    }

    /// <summary>
    /// Gets the method name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the parent module.
    /// </summary>
    public ComancheModule Parent { get; }

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
            .Select(p => callParams.ContainsKey(p) ? p.ParseInput(callParams[p]) : null)
            .ToArray();

        return this.call(this.Parent.Type, parameterVals);
    }
}