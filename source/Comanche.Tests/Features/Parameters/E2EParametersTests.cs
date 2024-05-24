// <copyright file="E2EParametersTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Features.Parameters;

using System;
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

    [Fact]
    public void Parameters_MissingRequired_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz sum-hash-set";
        const string expectedText = "--n: missing";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }

    [Fact]
    public void Parameters_CorrectDI_WritesExpected()
    {
        // Arrange
        const string command = "paramz correct-di";
        const string expectedText = "woot";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, false, null, false));
    }

    [Fact]
    public void Parameters_IncorrectDI_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz incorrect-di";
        const string expectedText = "--console: injected parameter must be hidden";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }

    [Fact]
    public void Parameters_SupplyHiddenParam_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz correct-di --console 123";
        const string expectedText = "--console: unrecognised";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }

    [Fact]
    public void Parameters_SkipOptionalParam_ReturnsExpected()
    {
        // Arrange
        const string command = "paramz sum-optional-array";
        const int expected = -1;

        // Act
        var result = E2E.Run(command);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Parameters_OptionalFalsyBoolButWithFlagName_EffectsTrue()
    {
        // Arrange
        const string command = "paramz optional-bool --force";
        var expected = true.ToString();

        // Act
        var result = E2E.Run(command);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Monday")]
    [InlineData("1")]
    public void Parameters_EnumInputs_ReturnExpected(string param)
    {
        // Arrange
        var command = $"paramz next-day --day {param}";
        const DayOfWeek expected = DayOfWeek.Tuesday;

        // Act
        var result = E2E.Run(command);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Parameters_BadEnumInput_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz next-day --day Funday";
        const string expectedText = "--day: not in enum";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }
}
