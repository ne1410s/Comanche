// <copyright file="ConsoleWriterTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Services;

using System;
using System.IO;
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
        sut.WriteLine("foo", true);
        sut.WriteLine("foo", true);

        // Assert
        writer.ToString().Should().Contain("foo" + Environment.NewLine);
        sut.Counter["Red"].Should().Be(2);
    }

    [Fact]
    public void WriteLine_NotError_WritesToStandardStream()
    {
        // Arrange
        StringWriter writer = new();
        ConsoleWriter sut = new();
        Console.SetOut(writer);

        // Act
        sut.WriteLine("bar", false);
        sut.WriteLine("bar", false);

        // Assert
        writer.ToString().Should().Contain("bar" + Environment.NewLine);
        sut.Counter["White"].Should().Be(2);
    }
}