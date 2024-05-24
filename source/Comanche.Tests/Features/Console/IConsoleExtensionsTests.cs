// <copyright file="IConsoleExtensionsTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Features.Console;

using System;
using System.Collections.ObjectModel;

/// <summary>
/// Tests for the <see cref="IConsoleExtensions"/> class.
/// </summary>
public class IConsoleExtensionsTests
{
    [Fact]
    public void CaptureString_WhenCalled_InvokesInterfaceMethod()
    {
        // Arrange
        var mockConsole = new Mock<IConsole>();
        const string input = "hi";
        const char mask = '*';

        // Act
        mockConsole.Object.CaptureString(input, mask);

        // Assert
        mockConsole.Verify(m => m.CaptureStrings(input, mask, 1));
    }

    [Theory]
    [InlineData("1,2,3", "1")]
    [InlineData("", null)]
    public void CaptureString_VaryingResults_ReturnsFirstOrDefault(string itemCsv, string? expected)
    {
        // Arrange
        var mockConsole = new Mock<IConsole>();
        var items = itemCsv.Split(',', StringSplitOptions.RemoveEmptyEntries);
        mockConsole
            .Setup(m => m.CaptureStrings(It.IsAny<string>(), It.IsAny<char?>(), It.IsAny<byte>()))
            .Returns(new Collection<string>(items));

        // Act
        var result = mockConsole.Object.CaptureString();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void CaptureString_NullConsole_ThrowsException()
    {
        // Arrange
        var nullConsole = (IConsole)null!;

        // Act
        var act = () => nullConsole.CaptureString();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WriteStructured_AllNulls_WritesExpected()
    {
        // Arrange
        var sut = new PlainWriter();

        // Act
        sut.WriteStructured();

        // Assert
        sut.Text(true).Should().Be(string.Empty);
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
        sut.Text(true).Should().Be(expected);
    }
}
