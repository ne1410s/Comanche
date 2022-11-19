// <copyright file="RouteBuilderException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Exceptions;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents errors that occur during the building of a route.
/// </summary>
public class RouteBuilderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RouteBuilderException"/> class.
    /// </summary>
    /// <param name="deepestValidTerms">The deepest valid route terms.</param>
    public RouteBuilderException(IList<string> deepestValidTerms)
    {
        this.DeepestValidTerms = deepestValidTerms;
    }

    /// <summary>
    /// Gets the deepest valid route terms.
    /// </summary>
    public IList<string> DeepestValidTerms { get; }
}
