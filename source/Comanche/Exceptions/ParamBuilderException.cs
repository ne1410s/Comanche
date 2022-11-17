// <copyright file="ParamBuilderException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Exceptions;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents errors that occur during the building of parameters.
/// </summary>
public class ParamBuilderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParamBuilderException"/> class.
    /// </summary>
    /// <param name="errors">The errors.</param>
    public ParamBuilderException(IReadOnlyDictionary<string, string> errors)
        : base("Invalid parameters found.")
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Gets the errors.
    /// </summary>
    public IReadOnlyDictionary<string, string> Errors { get; }
}
