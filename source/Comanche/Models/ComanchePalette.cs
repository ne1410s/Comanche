// <copyright file="ComanchePalette.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;

/// <summary>
/// The supported colour structure.
/// </summary>
public class ComanchePalette
{
    /// <summary>
    /// Gets the default colour.
    /// </summary>
    public ConsoleColor Default { get; init; } = Console.ForegroundColor;

    /// <summary>
    /// Gets the primary colour.
    /// </summary>
    public ConsoleColor Primary { get; init; } = ConsoleColor.Yellow;

    /// <summary>
    /// Gets the secondary colour.
    /// </summary>
    public ConsoleColor Secondary { get; init; } = ConsoleColor.Blue;

    /// <summary>
    /// Gets the tertiary colour.
    /// </summary>
    public ConsoleColor Tertiary { get; init; } = ConsoleColor.DarkMagenta;

   /// <summary>
   /// Gets the error colour.
   /// </summary>
    public ConsoleColor Error { get; init; } = ConsoleColor.Red;
}
