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
    /// Writes text to the console.
    /// </summary>
    /// <param name="text">The text fragment to write.</param>
    /// <param name="line">Whether to terminate with a new line.</param>
    /// <param name="colour">The colour.</param>
    public void Write(string text, bool line = false, ConsoleColor? colour = null);

    /// <summary>
    /// Writes text in the <see cref="ComanchePalette.Primary"/> colour.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="line">Whether to terminate with a new line.</param>
    public void WritePrimary(string text, bool line = false);

    /// <summary>
    /// Writes text in the <see cref="ComanchePalette.Secondary"/> colour.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="line">Whether to terminate with a new line.</param>
    public void WriteSecondary(string text, bool line = false);

    /// <summary>
    /// Writes text in the <see cref="ComanchePalette.Tertiary"/> colour.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="line">Whether to terminate with a new line.</param>
    public void WriteTertiary(string text, bool line = false);

    /// <summary>
    /// Writes error text in the <see cref="ComanchePalette.Error"/> colour.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="line">Whether to terminate with a new line.</param>
    public void WriteError(string text, bool line = false);

    /// <summary>
    /// Captures a single string.
    /// </summary>
    /// <param name="prompt">The prompt text.</param>
    /// <param name="mask">Optional mask character.</param>
    /// <returns>The result.</returns>
    public string CaptureString(string prompt = "Input: ", char? mask = null);

    /// <summary>
    /// Captures multiple strings.
    /// </summary>
    /// <param name="prompt">The prompt text.</param>
    /// <param name="mask">Optional mask character.</param>
    /// <param name="max">The maximum number of strings to capture.</param>
    /// <returns>The result.</returns>
    public Collection<string> CaptureStrings(string prompt = "Input: ", char? mask = null, byte max = 255);
}