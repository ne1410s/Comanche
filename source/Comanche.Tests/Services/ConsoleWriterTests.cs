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
    public void ConsoleBackspace_WhenCalled_ReturnsExpected()
    {
        // Arrange
        var sut = new ConsoleWriter();
        const string expected = "\b \b";

        // Act
        var actual = sut.ConsoleBackspace;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Write_IsError_WritesToErrorStream()
    {
        // Arrange
        StringWriter writer = new();
        ConsoleWriter sut = new();
        Console.SetError(writer);

        // Act
        sut.Write("foo", WriteStyle.Error);

        // Assert
        writer.ToString().Should().Contain("foo");
    }

    [Fact]
    public void Write_NotError_WritesToStandardStream()
    {
        // Arrange
        StringWriter writer = new();
        ConsoleWriter sut = new();
        Console.SetOut(writer);

        // Act
        sut.Write("bar", WriteStyle.Default);

        // Assert
        writer.ToString().Should().Contain("bar");
    }

    [Theory]
    [InlineData("bar", WriteStyle.Default, true)]
    [InlineData("bar", WriteStyle.Highlight1, true)]
    [InlineData("bar", WriteStyle.Highlight2, true)]
    [InlineData("bar", WriteStyle.Highlight3, false)]
    [InlineData("bar", WriteStyle.Error, true)]
    public void Write_VaryingParams_TracksAccordingly(string text, WriteStyle style, bool line)
    {
        // Arrange
        ConsoleWriter sut = new();
        var expected = Tuple.Create(line ? text + Environment.NewLine : text, style, line);

        // Act
        sut.Write(text, style, line);

        // Assert
        sut.LastCommand.Should().Be(expected);
    }

    [Fact]
    public void WriteStructured_VaryingParams_OutputsExpected()
    {
        // Arrange
        StringWriter writer = new();
        ConsoleWriter sut = new();
        Console.SetOut(writer);
        var expected = "mynameisstan" + Environment.NewLine;

        // Act
        sut.WriteStructured("my", "name", "is", "stan");

        // Assert
        writer.ToString().Should().Be(expected);
    }
}