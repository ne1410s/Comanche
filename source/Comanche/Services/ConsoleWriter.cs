// <copyright file="ConsoleWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IOutputWriter"/>
public class ConsoleWriter : IOutputWriter
{
    private readonly Dictionary<ConsoleColor, List<string>> log = new()
    {
        { ConsoleColor.Red, new List<string>() },
        { ConsoleColor.White, new List<string>() },
    };

    /// <summary>
    /// Gets a list of standard entries, most recent first.
    /// </summary>
    public IReadOnlyList<string> Entries => this.log[ConsoleColor.White];

    /// <summary>
    /// Gets a list of error entries, most recent first.
    /// </summary>
    public IReadOnlyList<string> ErrorEntries => this.log[ConsoleColor.Red];

    /// <inheritdoc/>
    public void WriteLine(string text, bool isError = false) =>
        this.WriteLineInternal(text, isError);

    private void WriteLineInternal(string text, bool error)
    {
        ConsoleColor priorForeground = Console.ForegroundColor;
        ConsoleColor foreground = error ? ConsoleColor.Red : ConsoleColor.White;
        Console.ForegroundColor = foreground;
        if (error)
        {
            Console.Error.WriteLine(text);
        }
        else
        {
            Console.WriteLine(text);
        }

        this.log[foreground].Insert(0, text);
        Console.ForegroundColor = priorForeground;
    }
}