// <copyright file="ConsoleWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IOutputWriter"/>
public class ConsoleWriter : IOutputWriter
{
    /// <inheritdoc/>
    public void WriteLine(string text, bool isError = false) =>
        this.WriteLineInternal(text, isError);

    private void WriteLineInternal(string text, bool error)
    {
        ConsoleColor priorForeground = Console.ForegroundColor;
        Console.ForegroundColor = error ? ConsoleColor.Red : ConsoleColor.White;
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