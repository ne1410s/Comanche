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
    public void Write(string? text = null, WriteStyle style = WriteStyle.Default, bool line = false)
        => this.WriteInternal(text, style, line);

    /// <inheritdoc/>
    public void WriteStructured(string? prefix = null, string? main = null, string? extra = null, string? suffix = null)
    {
        this.Write(prefix, WriteStyle.Highlight1);
        this.Write(main);
        this.Write(extra, WriteStyle.Highlight2);
        this.Write(suffix, WriteStyle.Highlight3);
        this.Write(line: true);
    }

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

    private void WriteInternal(string? text, WriteStyle style, bool line)
    {
        ConsoleColor priorForeground = Console.ForegroundColor;

        Action<string> write = style == WriteStyle.Error ? Console.Error.Write : Console.Write;
        Console.ForegroundColor = style switch
        {
            WriteStyle.Highlight1 => ConsoleColor.Yellow,
            WriteStyle.Highlight2 => ConsoleColor.Blue,
            WriteStyle.Highlight3 => ConsoleColor.DarkMagenta,
            WriteStyle.Error => ConsoleColor.Red,
            _ => priorForeground,
        };

        var actual = text + (line ? Environment.NewLine : string.Empty);
        write(actual);
        this.LastCommand = new(actual, style, line);

        Console.ForegroundColor = priorForeground;
    }
}