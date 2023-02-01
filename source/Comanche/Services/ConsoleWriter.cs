// <copyright file="ConsoleWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

/// <inheritdoc cref="IOutputWriter"/>
public class ConsoleWriter : IOutputWriter
{
    /// <summary>
    /// Gets the error count.
    /// </summary>
    public Dictionary<string, int> Counter { get; } = new();

    /// <inheritdoc/>
    public void WriteLine(string text, bool isError = false) =>
        this.WriteLineInternal(text, isError);

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

    private void WriteLineInternal(string text, bool error)
    {
        ConsoleColor priorForeground = Console.ForegroundColor;
        var color = error ? ConsoleColor.Red : ConsoleColor.White;
        Console.ForegroundColor = color;
        this.Counter[$"{color}"] = this.Counter.TryGetValue($"{color}", out var val) ? val + 1 : 1;
        if (error)
        {
            Console.Error.WriteLine(text);
        }
        else
        {
            Console.WriteLine(text);
        }

        Console.ForegroundColor = priorForeground;
    }
}