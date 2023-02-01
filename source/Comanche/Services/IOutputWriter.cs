// <copyright file="IOutputWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services;

using System.Collections.Generic;

/// <summary>
/// Writes output.
/// </summary>
public interface IOutputWriter
{
    /// <summary>
    /// Outputs a line of text.
    /// </summary>
    /// <param name="text">The line of text.</param>
    /// <param name="isError">True if the text represents an error.</param>
    public void WriteLine(string text, bool isError = false);

    /// <summary>
    /// Captures multiple strings.
    /// </summary>
    /// <param name="prompt">The prompt text.</param>
    /// <returns>The result.</returns>
    public List<string> CaptureStrings(string prompt = "Input: ");
}