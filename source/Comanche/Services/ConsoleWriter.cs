// <copyright file="ConsoleWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Comanche.Models;

/// <inheritdoc cref="IOutputWriter"/>
public class ConsoleWriter : IOutputWriter
{
    /// <summary>
    /// Gets the most recent command.
    /// </summary>
    public Tuple<string, WriteStyle, bool>? LastCommand { get; private set; }

    /// <summary>
    /// Effects a console backspace.
    /// </summary>
    [Localizable(false)]
    public string ConsoleBackspace => "\b \b";

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
    public Collection<string> CaptureStrings(string prompt = "Input: ", char? mask = null, byte max = 255)
    {
        Console.Write(prompt);
        var retVal = new Collection<string>();
        var lineBuilder = new StringBuilder();

        for (var capture = 0; capture < max; capture++)
        {
            lineBuilder.Clear();
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;
                if (key == ConsoleKey.Backspace && lineBuilder.Length > 0)
                {
                    Console.Write(this.ConsoleBackspace);
                    lineBuilder.Remove(lineBuilder.Length - 1, 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write(mask ?? keyInfo.KeyChar);
                    lineBuilder.Append(keyInfo.KeyChar);
                }
            }
            while (key != ConsoleKey.Enter);
            if (lineBuilder.Length == 0)
            {
                break;
            }

            retVal.Add(lineBuilder.ToString());
            Console.WriteLine();
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