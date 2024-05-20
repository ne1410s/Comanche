// <copyright file="ComanchePalette.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;

/// <summary>
/// The supported colour structure.
/// </summary>
public record ComanchePalette(
    ConsoleColor Primary = ConsoleColor.Yellow,
    ConsoleColor Secondary = ConsoleColor.Blue,
    ConsoleColor Tertiary = ConsoleColor.DarkMagenta,
    ConsoleColor Error = ConsoleColor.Red)
{
    /// <summary>
    /// Gets the default colour.
    /// </summary>
    public ConsoleColor Default { get; init; } = Console.ForegroundColor;
}
