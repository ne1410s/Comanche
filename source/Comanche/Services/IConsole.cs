// <copyright file="IConsole.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services;

using System;
using System.Collections.ObjectModel;
using Comanche.Models;

/// <summary>
/// Console behaviour.
/// </summary>
public interface IConsole
{
    /// <summary>
    /// Gets the palette.
    /// </summary>
    public ComanchePalette Palette { get; }

    /// <summary>
    /// Writes text to the console.
    /// </summary>
    /// <param name="text">The text fragment to write.</param>
    /// <param name="line">Whether to terminate with a new line.</param>
    /// <param name="colour">The colour.</param>
    /// <param name="err">Whether to write as error.</param>
    public void Write(string? text = null, bool line = false, ConsoleColor? colour = null, bool err = false);

    /// <summary>
    /// Captures multiple strings.
    /// </summary>
    /// <param name="prompt">The prompt text.</param>
    /// <param name="mask">Optional mask character.</param>
    /// <param name="max">The maximum number of strings to capture.</param>
    /// <returns>The result.</returns>
    public Collection<string> CaptureStrings(string prompt = "Input: ", char? mask = null, byte max = 255);
}