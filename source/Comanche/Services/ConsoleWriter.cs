// <copyright file="ConsoleWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Comanche.Models;

/// <inheritdoc cref="IOutputWriter"/>
public class ConsoleWriter : IOutputWriter
{
    /// <summary>
    /// Gets the most recent command.
    /// </summary>
    public Tuple<string, WriteStyle, bool>? LastCommand { get; private set; }

    /// <inheritdoc/>
    public void Write(string text, WriteStyle style = WriteStyle.Default)
        => this.WriteInternal(text, style, false);

    /// <inheritdoc/>
    public void WriteLine(string text, WriteStyle style = WriteStyle.Default)
        => this.WriteInternal(text, style, true);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public Collection<string> CaptureStrings(string prompt = "Input: ")
    {
        Console.Write(prompt);
        var retVal = new Collection<string>();
        while (true)
        {
            var read = Console.ReadLine();
            if (!string.IsNullOrEmpty(read))
            {
                retVal.Add(read);
            }
            else
            {
                break;
            }
        }

        return retVal;
    }

    private void WriteInternal(string text, WriteStyle style, bool line)
    {
        Action<string> write = new { style, line } switch
        {
            { style: WriteStyle.Error, line: true } => Console.Error.WriteLine,
            { style: WriteStyle.Error, line: false } => Console.Error.Write,
            { line: false } => Console.Write,
            _ => Console.WriteLine,
        };

        ConsoleColor priorForeground = Console.ForegroundColor;
        Console.ForegroundColor = style switch
        {
            WriteStyle.Highlight1 => ConsoleColor.Yellow,
            WriteStyle.Highlight2 => ConsoleColor.Blue,
            WriteStyle.Highlight3 => ConsoleColor.DarkMagenta,
            WriteStyle.Error => ConsoleColor.Red,
            _ => priorForeground,
        };

        write(text);
        this.LastCommand = new(text, style, line);

        Console.ForegroundColor = priorForeground;
    }
}