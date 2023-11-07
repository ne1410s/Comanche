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
    public void Write(string text, WriteStyle style = WriteStyle.Default);

    /// <summary>
    /// Outputs a line of text.
    /// </summary>
    /// <param name="text">The line of text.</param>
    /// <param name="style">The output writing style.</param>
    public void WriteLine(string text, WriteStyle style = WriteStyle.Default);

    /// <summary>
    /// Captures multiple strings.
    /// </summary>
    /// <param name="prompt">The prompt text.</param>
    /// <returns>The result.</returns>
    public Collection<string> CaptureStrings(string prompt = "Input: ");
}