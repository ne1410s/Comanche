// <copyright file="ConsoleWriterTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Services;

using System;
using System.IO;
using Comanche.Models;
using Comanche.Services;

/// <summary>
/// Tests for the <see cref="ConsoleWriter"/> class.
/// </summary>
public class ConsoleWriterTests
{
    [Fact]
    public void WriteLine_IsError_WritesToErrorStream()
    {
        // Arrange
        StringWriter writer = new();
        ConsoleWriter sut = new();
        Console.SetError(writer);

        // Act
        sut.WriteLine("foo", WriteStyle.Error);

        // Assert
        writer.ToString().Should().Contain("foo" + Environment.NewLine);
    }

    [Fact]
    public void WriteLine_NotError_WritesToStandardStream()
    {
        // Arrange
        StringWriter writer = new();
        ConsoleWriter sut = new();
        Console.SetOut(writer);

        // Act
        sut.WriteLine("bar", WriteStyle.Default);

        // Assert
        writer.ToString().Should().Contain("bar" + Environment.NewLine);
    }

    [Theory]
    [InlineData("foo", WriteStyle.Default)]
    [InlineData("foo", WriteStyle.Highlight1)]
    [InlineData("foo", WriteStyle.Highlight2)]
    [InlineData("foo", WriteStyle.Highlight3)]
    [InlineData("foo", WriteStyle.Error)]
    public void WriteLine_VaryingParams_TracksAccordingly(string text, WriteStyle style)
    {
        // Arrange
        ConsoleWriter sut = new();

        // Act
        sut.WriteLine(text, style);

        // Assert
        sut.LastCommand.Should().Be(Tuple.Create(text, style, true));
    }

    [Theory]
    [InlineData("bar", WriteStyle.Default)]
    [InlineData("bar", WriteStyle.Highlight1)]
    [InlineData("bar", WriteStyle.Highlight2)]
    [InlineData("bar", WriteStyle.Highlight3)]
    [InlineData("bar", WriteStyle.Error)]
    public void Write_VaryingParams_TracksAccordingly(string text, WriteStyle style)
    {
        // Arrange
        ConsoleWriter sut = new();

        // Act
        sut.Write(text, style);

        // Assert
        sut.LastCommand.Should().Be(Tuple.Create(text, style, false));
    }
}