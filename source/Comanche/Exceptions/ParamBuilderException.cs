// <copyright file="ParamBuilderException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Exceptions;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents errors that occur during the building of parameters.
/// </summary>
internal sealed class ParamBuilderException(IReadOnlyDictionary<string, string> errors) : Exception
{
    /// <summary>
    /// Gets the errors.
    /// </summary>
    public IReadOnlyDictionary<string, string> Errors { get; } = errors;
}
