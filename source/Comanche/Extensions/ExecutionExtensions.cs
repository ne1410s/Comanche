// <copyright file="ExecutionExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using System.Collections.Generic;
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
    public static object? Execute(this ComancheMethod method, IList<string> parameters)
    {
        // TODO!

        throw new NotImplementedException();
    }
}
