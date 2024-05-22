// <copyright file="ConsoleWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

/// <inheritdoc cref="IConsole"/>
public class ConsoleWriter(ComanchePalette palette) : IConsole
{
    /// <inheritdoc/>
    public ComanchePalette Palette { get; } = palette;

    /// <summary>
    /// Gets the most recent command.
    /// </summary>
    public Tuple<string, bool, ConsoleColor, bool>? LastCommand { get; private set; }

    /// <summary>
    /// Gets a console backspace.
    /// </summary>
    [Localizable(false)]
    public string ConsoleBackspace => "\b \b";

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
    public void Write(string? text = null, bool line = false, ConsoleColor? colour = null, bool err = false)
    {
        var actualText = text + (line ? Environment.NewLine : string.Empty);
        var priorForeground = Console.ForegroundColor;
        var actualColour = colour ?? palette.Default;
        Console.ForegroundColor = actualColour;
        Action<string> write = err ? Console.Error.Write : Console.Write;
        write(actualText);
        this.LastCommand = new(actualText, line, actualColour, err);
        Console.ForegroundColor = priorForeground;
    }
}