// <copyright file="PlainWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Features.Console;

using System;
using System.Collections.ObjectModel;
using System.Text;

internal sealed class PlainWriter : IConsole
{
    private readonly StringBuilder sb = new();

    public ComanchePalette Palette { get; } = new();

    public override string ToString() => this.sb.ToString();

    public Collection<string> CaptureStrings(string prompt = "Input: ", char? mask = null, byte max = 255) => null!;

    public void Write(string? text = null, bool line = false, ConsoleColor? colour = null, bool err = false)
        => this.sb.Append(text).Append(line ? Environment.NewLine : string.Empty);

    public string Text(bool? normaliseAllSpaces = null)
    {
        var text = this.ToString();
        return normaliseAllSpaces switch
        {
            null => text,
            _ => text.Normalise(normaliseAllSpaces.Value),
        };
    }
}
