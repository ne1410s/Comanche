// <copyright file="E2EParametersTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Parameters;

using System;
using Comanche.Tests.Console;
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
    public void Parameters_ValidGenericInterfaceSequence_ReturnExpected()
    {
        // Arrange
        const string command = "paramz sum-generic-interface --n 3 --n 5";
        const int expected = 8;

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
    public void Parameters_MultipleJsonArrayInputs_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz sum-hash-set --n [ 2, 3 ] --n [ 3, 4 ]";
        const string expectedText = "--n: cannot convert";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }

    [Fact]
    public void Parameters_InconvertibleMemberInArray_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz sum-optional-array --n 1 --n toast --n 3";
        const string expectedText = "--n: cannot convert";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }

    [Fact]
    public void Parameters_InconvertibleMemberInSequence_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz sum-hash-set --n 1 --n toast --n 3";
        const string expectedText = "--n: cannot convert";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }

    [Fact]
    public void Parameters_InconstructibleSequence_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz sum-bad-hash-set --n 1 --n 2 --n 3";
        const string expectedText = "--n: cannot create sequence";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);
        E2EParametersModule.SumBadHashSet([]);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }

    [Fact]
    public void Parameters_MultipleInputsOnNonArray_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz next-day --day 1 --day 2";
        const string expectedText = "--day (-d): not array";
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
    public void Parameters_EmptyNullishParam_SendsNull()
    {
        // Arrange
        const string command = "paramz sum-optional-array --n";
        const int expected = -1;

        // Act
        var actual = E2E.Run(command);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Parameters_EmptyNonBoolValueParam_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz next-day --day";
        const string expectedText = "--day (-d): missing";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);
        E2EParametersModule.IncorrectDI(mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }

    [Fact]
    public void Parameters_CorrectDI_WritesExpectedVerbatim()
    {
        // Arrange
        const string command = "paramz correct-di";
        const string expectedText = "woot\n";
        var plainWriter = new PlainWriter();

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(false).Should().Be(expectedText);
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
        E2EParametersModule.IncorrectDI(mockConsole.Object);

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

    [Theory]
    [InlineData("paramz sum-optional-array", -1)]
    [InlineData("paramz sum-optional-array --n 3 --n 4", 7)]
    public void Parameters_OptionalParam_ReturnsExpected(string command, int expected)
    {
        // Arrange & Act
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
    [InlineData("--day Monday")]
    [InlineData("-d 1")]
    public void Parameters_EnumInputs_ReturnExpected(string param)
    {
        // Arrange
        var command = $"paramz next-day {param}";
        const DayOfWeek expected = DayOfWeek.Tuesday;

        // Act
        var result = E2E.Run(command);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Parameters_OutputtingNestedEnum_JsonHasEnumText()
    {
        // Arrange
        const string command = "paramz next-day-obj -d wEdNesDAy";
        const string expected = """
        {
          "day": "Thursday"
        }
        """;
        var mockConsole = E2E.DefaultPalette.GetMockConsole();

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expected, true, null, false));
    }

    [Fact]
    public void Parameters_BadEnumInput_WritesExpectedError()
    {
        // Arrange
        const string command = "paramz next-day --day Funday";
        const string expectedText = "--day (-d): not in enum";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        var expectedColour = E2E.DefaultPalette.Error;

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expectedText, true, expectedColour, true));
    }

    [Fact]
    public void Parameters_ValidGuid_ParsedOk()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var command = $"paramz reformat-guid --id {guid}";
        var expected = guid.ToString("P");

        // Act
        var actual = E2E.Run(command);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Parameters_InvalidGuid_WritesExpectedErrorVerbatim()
    {
        // Arrange
        const string command = "paramz reformat-guid --id n0t-4-gU1D";
        var expectedText = """
            
            Invalid Parameters:
            --id: cannot parse guid

            Note:
            Run again with --help for a full parameter list.


            """.Normalise(false);
        var plainWriter = new PlainWriter();

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        var actualText = plainWriter.Text(false);
        actualText.Should().Be(expectedText);
    }

    [Fact]
    public void Parameters_Nullables_ParseAsExpected()
    {
        // Arrange
        const string command = "paramz nullables --num 32 --str hi";
        const string expected = "hi=32";

        // Act
        var result = E2E.Run(command);

        // Assert
        result.Should().Be(expected);
    }
}
