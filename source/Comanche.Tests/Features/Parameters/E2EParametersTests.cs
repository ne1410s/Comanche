// <copyright file="E2EParametersTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Features.Parameters;

using E2E = TestHelper;

public class E2EParametersTests
{
    [Fact]
    public void Parameters_ValidComplexInputType_ReturnsExpected()
    {
        // Arrange
        const string command = "paramz sum-dicto-values --dicto { \"a\": 1, \"b\": 2 }";
        const int expected = 3;

        // Act
        var actual = E2E.Run(command);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Parameters_InvalidComplexInputType_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz sum-dicto-values --dicto { borkedL:2 + 2 }";
        const string expectedText = "--dicto: cannot deserialise";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }

    [Theory]
    [InlineData("--n 4 --n 5 --n 6", 15)]
    [InlineData("--n [2, 3, 7]", 12)]
    public void Parameters_ValidSequenceFormats_ReturnExpected(string param, int expected)
    {
        // Arrange
        var command = $"paramz sum-hash-set {param}";

        // Act
        var actual = E2E.Run(command);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Parameters_InvalidSequenceFormat_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz sum-hash-set --n [ why! ]";
        const string expectedText = "--n: cannot deserialise";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }
}
