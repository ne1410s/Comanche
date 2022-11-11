﻿// <copyright file="ExecutionExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;
using System.Threading.Tasks;
using Comanche.Models;

/// <summary>
/// Extensions relating to method invocation.
/// </summary>
public static class ExecutionExtensions
{
    /// <summary>
    /// Executes a method.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The result.</returns>
    public static async Task<object?> ExecuteAsync(
        this ComancheMethod method,
        object?[] parameters)
    {
        // TODO: Wrap in exception handling, etc.

        return await method.CallAsync(parameters);
    }
}
