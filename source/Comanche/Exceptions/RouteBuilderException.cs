// <copyright file="RouteBuilderException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Exceptions;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents errors that occur during the building of a route.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RouteBuilderException"/> class.
/// </remarks>
/// <param name="deepestValidTerms">The deepest valid route terms.</param>
/// <param name="message">The message.</param>
internal sealed class RouteBuilderException(IList<string> deepestValidTerms, string message) : Exception(message)
{
    /// <summary>
    /// Gets the deepest valid route terms.
    /// </summary>
    public IList<string> DeepestValidTerms { get; } = deepestValidTerms;
}
