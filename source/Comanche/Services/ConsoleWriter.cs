// <copyright file="ConsoleWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Comanche.Models;
using Microsoft.Extensions.Options;

/// <inheritdoc cref="IConsole"/>
public class ConsoleWriter(IOptions<ComanchePalette> paletteOptions) : IConsole
{
    private readonly ComanchePalette palette

    /// <summary>
    /// Gets the most recent command.
    /// </summary>
    public Tuple<string, bool, ConsoleColor>? LastCommand { get; private set; }

    /// <summary>
    /// Gets a console backspace.
    /// </summary>
    [Localizable(false)]
    public string ConsoleBackspace => "\b \b";

    /// <inheritdoc/>
    public string CaptureString(string prompt = "Input: ", char? mask = null) =>
        this.CaptureStrings(prompt, mask, 1).SingleOrDefault();

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

    /// <inheritdoc/>
    public void Write(string text, bool line = false, ConsoleColor? colour = null)
    {
        var actualText = text + (line ? Environment.NewLine : string.Empty);
        var actualColour = colour ?? Console.ForegroundColor;
        ConsoleColor priorForeground = Console.ForegroundColor;
        Console.ForegroundColor = actualColour;
        Console.Write(actualText);
        this.LastCommand = new(actualText, line, actualColour);
        Console.ForegroundColor = priorForeground;
    }

    /// <inheritdoc/>
    public void WritePrimary(string text, bool line = false) => this.Write(text, line, palette.Value.Primary)

    /// <inheritdoc/>
    public void WriteSecondary(string text, bool line = false) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void WriteTertiary(string text, bool line = false) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void WriteError(string text, bool line = false) => throw new NotImplementedException();

    private void WriteInternal(string? text, ConsoleColor colour, bool line)
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