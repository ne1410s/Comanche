// <copyright file="ConsoleExtensionsTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Console;

using System;
using System.Collections.ObjectModel;

/// <summary>
/// Tests for the <see cref="ConsoleExtensions"/> class.
/// </summary>
public class ConsoleExtensionsTests
{
    [Fact]
    public void CaptureString_WhenCalled_InvokesInterfaceMethod()
    {
        // Arrange
        var mockConsole = new Mock<IConsole>();
        const string input = "hi";
        const char mask = '*';

        // Act
        _ = mockConsole.Object.CaptureString(input, mask);

        // Assert
        mockConsole.Verify(m => m.CaptureStrings(input, mask, 1));
    }

    [Theory]
    [InlineData("1,2,3", "1")]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void CaptureString_VaryingResults_ReturnsFirstOrDefault(string? itemCsv, string? expected)
    {
        // Arrange
        var mockConsole = new Mock<IConsole>();
        var items = (itemCsv ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries);
        _ = mockConsole
            .Setup(m => m.CaptureStrings(It.IsAny<string>(), It.IsAny<char?>(), It.IsAny<byte>()))
            .Returns(new Collection<string>(items));

        // Act
        var result = mockConsole.Object.CaptureString();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void CaptureString_NullConsole_ThrowsException()
    {
        // Arrange
        var nullConsole = (IConsole)null!;

        // Act
        var act = () => nullConsole.CaptureString();

        // Assert
        _ = act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void WriteLine_WhenCalled_InvokesInterfaceMethod()
    {
        // Arrange
        var mockConsole = new Mock<IConsole>();

        // Act
        mockConsole.Object.WriteLine();

        // Assert
        mockConsole.Verify(m => m.Write(string.Empty, true, null, false));
    }

    [Fact]
    public void WriteStructured_AllNulls_WritesExpected()
    {
        // Arrange
        var sut = new PlainWriter();

        // Act
        sut.WriteStructured();

        // Assert
        sut.Text(true).ShouldBe(string.Empty);
    }

    [Fact]
    public void WriteStructured_VaryingParams_OutputsExpected()
    {
        // Arrange
        var sut = new PlainWriter();
        const string expected = "mynameisstan";

        // Act
        sut.WriteStructured("my", "name", "is", "stan");

        // Assert
        sut.Text(true).ShouldBe(expected);
    }
}
