// <copyright file="ExecutionException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Exceptions;

using System;

/// <summary>
/// Represents errors that occur during execution.
/// </summary>
internal class ExecutionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="invocationStack">The invocation stack trace.</param>
    public ExecutionException(string message, Exception innerException, string? invocationStack = null)
        : base(message, innerException)
    {
        this.InvocationStack = invocationStack;
    }

    /// <summary>
    /// Gets the stack trace of invocation.
    /// </summary>
    public string? InvocationStack { get; }
}
