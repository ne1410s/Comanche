// <copyright file="ExecutionException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Exceptions;

using System;

/// <summary>
/// Represents errors that occur during execution.
/// </summary>
internal sealed class ExecutionException(string message, Exception innerException, string? invocationStack = null)
    : Exception(message, innerException)
{
    /// <summary>
    /// Gets the stack trace of invocation.
    /// </summary>
    public string? InvocationStack { get; } = invocationStack;
}
