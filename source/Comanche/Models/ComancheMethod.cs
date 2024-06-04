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
/// <param name="name">The method name.</param>
/// <param name="summary">The method summary.</param>
/// <param name="returns">The method returns.</param>
/// <param name="returnType">The return type.</param>
/// <param name="resolver">The caller resolver.</param>
/// <param name="call">The call function.</param>
/// <param name="parameters">The parameter definitions.</param>
internal sealed class ComancheMethod(
    string name,
    string? summary,
    string? returns,
    Type returnType,
    Func<object?> resolver,
    Func<object?, object?[], object?> call,
    List<ComancheParam> parameters)
{
    private readonly Func<object?, object?[], object?> call = call;
    private readonly Func<object?> resolver = resolver;

    /// <summary>
    /// Gets the method name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the method summary.
    /// </summary>
    public string? Summary { get; } = summary;

    /// <summary>
    /// Gets a list of parameters.
    /// </summary>
    public IReadOnlyList<ComancheParam> Parameters { get; } = parameters;

    /// <summary>
    /// Gets the method return type.
    /// </summary>
    public Type ReturnType { get; } = returnType;

    /// <summary>
    /// Gets the method returns.
    /// </summary>
    public string? Returns { get; } = returns;

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
        catch (Exception wrapperEx)
        {
            var ex = wrapperEx.InnerException ?? wrapperEx;
            throw new ExecutionException(ex.Message, ex, ex.StackTrace);
        }
    }
}