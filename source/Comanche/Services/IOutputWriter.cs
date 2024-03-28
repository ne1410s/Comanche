// <copyright file="IOutputWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services;

using System.Collections.ObjectModel;
using Comanche.Models;

/// <summary>
/// Writes output.
/// </summary>
public interface IOutputWriter
{
    /// <summary>
    /// Outputs a text fragment.
    /// </summary>
    /// <param name="text">The text fragment.</param>
    /// <param name="style">The output writing style.</param>
    /// <param name="line">Whether to append a new line.</param>
    public void Write(string? text = null, WriteStyle style = WriteStyle.Default, bool line = false);

    /// <summary>
    /// Writes a structured line.
    /// </summary>
    /// <param name="prefix">The prefix.</param>
    /// <param name="main">The main text.</param>
    /// <param name="extra">The extra text.</param>
    /// <param name="suffix">The suffix.</param>
    public void WriteStructured(
        string? prefix = null,
        string? main = null,
        string? extra = null,
        string? suffix = null);

    /// <summary>
    /// Captures multiple strings.
    /// </summary>
    /// <param name="prompt">The prompt text.</param>
    /// <param name="mask">Optional mask character.</param>
    /// <param name="max">The maximum number of strings to capture.</param>
    /// <returns>The result.</returns>
    public Collection<string> CaptureStrings(string prompt = "Input: ", char? mask = null, byte max = 255);
}