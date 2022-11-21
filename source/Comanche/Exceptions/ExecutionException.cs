// <copyright file="ExecutionException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Exceptions;

using System;

/// <summary>
/// Represents errors that occur during execution.
/// </summary>
public class ExecutionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionException"/> class.
    /// </summary>
    public ExecutionException()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public ExecutionException(string message)
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ExecutionException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
