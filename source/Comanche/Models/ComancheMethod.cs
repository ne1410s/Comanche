// <copyright file="ComancheMethod.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;
using System.Collections.Generic;
using Comanche.Exceptions;

/// <summary>
/// A modelled method.
/// </summary>
internal class ComancheMethod
{
    private readonly Func<object?, object?[], object?> call;
    private readonly Func<object?> resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComancheMethod"/> class.
    /// </summary>
    /// <param name="name">The method name.</param>
    /// <param name="summary">The method summary.</param>
    /// <param name="returns">The method returns.</param>
    /// <param name="returnType">The return type.</param>
    /// <param name="resolver">The caller resolver.</param>
    /// <param name="call">The call function.</param>
    /// <param name="parameters">The parameter definitions.</param>
    public ComancheMethod(
        string name,
        string? summary,
        string? returns,
        Type returnType,
        Func<object?> resolver,
        Func<object?, object?[], object?> call,
        List<ComancheParam> parameters)
    {
        this.resolver = resolver;
        this.call = call;
        this.Name = name;
        this.Summary = summary;
        this.Parameters = parameters;
        this.Returns = returns;
        this.ReturnType = returnType;
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
    /// Gets the method return type.
    /// </summary>
    public Type ReturnType { get; }

    /// <summary>
    /// Gets the method returns.
    /// </summary>
    public string? Returns { get; }

    /// <summary>
    /// Calls the function.
    /// </summary>
    /// <param name="parameters">The call parameters.</param>
    /// <returns>The result.</returns>
    /// <exception cref="ExecutionException">Execution error.</exception>
    public object? Call(object?[] parameters)
    {
        try
        {
            var caller = this.resolver();
            return this.call(caller, parameters);
        }
        catch (Exception ex)
        {
            throw new ExecutionException($"Error calling '{this.Name}': {ex.InnerException.Message}", ex);
        }
    }
}