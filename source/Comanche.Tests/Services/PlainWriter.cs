// <copyright file="PlainWriter.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Services;

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Comanche.Models;
using Comanche.Services;

internal sealed class PlainWriter : IOutputWriter
{
    private static readonly Regex WhiteSpace = new(@"\s+");
    private readonly StringBuilder sb = new();

    public Collection<string> CaptureStrings(string prompt = "Input: ") => null!;

    public void Write(string? text = null, WriteStyle style = WriteStyle.Default, bool line = false)
        => this.sb.Append(text).Append(line ? Environment.NewLine : string.Empty);

    public void WriteStructured(string? prefix = null, string? main = null, string? extra = null, string? suffix = null)
        => this.Write(prefix + main + extra + suffix, line: true);

    public void ShouldBe(string expected)
    {
        var actual = WhiteSpace.Replace(this.sb.ToString(), " ");
        expected = WhiteSpace.Replace(expected, " ");
        actual.Should().Be(expected);
    }

    public void ShouldContain(string expected)
    {
        var actual = WhiteSpace.Replace(this.sb.ToString(), " ");
        expected = WhiteSpace.Replace(expected, " ");
        actual.Should().Contain(expected);
    }

    public void ShouldMatchVerbatim(string expected)
    {
        var actual = this.sb.ToString().Replace("\r\n", "\n", StringComparison.Ordinal);
        expected = expected.Replace("\r\n", "\n", StringComparison.Ordinal);
        actual.Should().Be(expected);
    }
}
