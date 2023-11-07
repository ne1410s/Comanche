// <copyright file="WriteStyle.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

/// <summary>
/// The style to write.
/// </summary>
public enum WriteStyle
{
    /// <summary>
    /// Writes to stdout in default colour.
    /// </summary>
    Default,

    /// <summary>
    /// Writes to stdout highlight #1 colour.
    /// </summary>
    Highlight1,

    /// <summary>
    /// Writes to stdout highlight #2 colour.
    /// </summary>
    Highlight2,

    /// <summary>
    /// Writes to stdout highlight #3 colour.
    /// </summary>
    Highlight3,

    /// <summary>
    /// Writes to stderr in red colour.
    /// </summary>
    Error,
}
