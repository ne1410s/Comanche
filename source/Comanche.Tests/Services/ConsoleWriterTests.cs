// <copyright file="ConsoleWriterTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Services;

using System;
using System.IO;

/// <summary>
/// Tests for the <see cref="ConsoleWriter"/> class.
/// </summary>
public class ConsoleWriterTests
{
    private static readonly ComanchePalette StandardPalette = new();

    [Fact]
    public void ConsoleBackspace_WhenCalled_ReturnsExpected()
    {
        // Arrange
        var sut = new ConsoleWriter(StandardPalette);
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
        ConsoleWriter sut = new(StandardPalette);
        Console.SetError(writer);

        // Act
        sut.WriteError("foo");

        // Assert
        writer.ToString().Should().Contain("foo");
    }

    [Fact]
    public void Write_NotError_WritesToStandardStream()
    {
        // Arrange
        StringWriter writer = new();
        ConsoleWriter sut = new(StandardPalette);
        Console.SetOut(writer);

        // Act
        sut.Write("bar");

        // Assert
        writer.ToString().Should().Contain("bar");
    }
}