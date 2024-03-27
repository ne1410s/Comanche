// <copyright file="DiscoverE2ETests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests;

using System;
using System.Reflection;
using Comanche.Models;
using Comanche.Services;
using Comanche.Tests.Services;

public class DiscoverE2ETests
{
    [Fact]
    public void Discover_AltParams_ReturnsExpected()
    {
        // Act
        var result = Discover.Go(true);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Discover_StringArray_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented join-array --s hello --s world";

        // Act
        var result = Invoke(command);

        // Assert
        result.Should().BeEquivalentTo("hello, world!");
    }

    [Fact]
    public void Discover_IntArray_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented sum-array --n 3 --n 4 --n 1";

        // Act
        var result = Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(8);
    }

    [Fact]
    public void Discover_IntList_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented sum -n 3 -n 4 -n 1 --n 0";

        // Act
        var result = Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(20);
    }

    [Fact]
    public void Discover_NullableWithValue_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented next --b 110";

        // Act
        var result = Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(111);
    }

    [Fact]
    public void Discover_NullableWithFlag_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented next --b";

        // Act
        var result = Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(256);
    }

    [Fact]
    public void Discover_GoodJson_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented sum-dicto --d { \"a\": 1, \"b\": 2 }";

        // Act
        var result = Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(3);
    }

    [Fact]
    public void Discover_ComplexTypeDefault_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented sum-dicto";

        // Act
        var result = Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(0);
    }

    [Fact]
    public void Discover_ReturnBareTask_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented static delay --ms 1";

        // Act
        var result = Invoke(command);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Discover_IntList_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented sum -n 3 -n 4 -n 1 --n 0";
        const string expected = "20";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        _ = Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Default, true));
    }

    [Fact]
    public void Discover_EmptyCommand_DoesNotWriteError()
    {
        // Arrange
        const string command = "";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(
            m => m.Write(It.IsAny<string>(), WriteStyle.Error, It.IsAny<bool>()),
            Times.Never());
    }

    [Theory]
    [InlineData("e2e")]
    [InlineData("e2e commented static single-mod")]
    public void Discover_BareModule_DoesNotWriteError(string command)
    {
        // Arrange
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(
            m => m.Write(It.IsAny<string>(), WriteStyle.Error, It.IsAny<bool>()),
            Times.Never());
    }

    [Fact]
    public void Discover_BareModule_WritesExpectedText()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented static single-mod";
        const string expected = @"
            Module: Comanche.Tests e2e commented static single-mod
            Methods: Comanche.Tests e2e commented static single-mod do
        ";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldBe(expected);
    }

    [Fact]
    public void Discover_Version_WritesExpected()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        var version = Assembly.GetAssembly(typeof(Discover))!.GetName().Version!.ToString(3);
        var year = DateTime.Today.Year;
        const string command = "--version";
        var expected = $@"
Module:
Comanche.Tests v1.0.0 (Test project)

CLI-ifier:
Comanche v{version} (ne1410s © {year})

";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldMatchVerbatim(expected);
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("--debug")]
    public void Discover_VersionPlusIncompatibleFlag_WritesExpectedError(string incompatibleFlag)
    {
        // Arrange
        var plainWriter = new PlainWriter();
        var command = "--version " + incompatibleFlag;

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldContain("--version: Command does not support --debug or --help");
    }

    [Fact]
    public void Discover_ImmediateHelp_DoesNotWriteError()
    {
        // Arrange
        const string command = "--help";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(
            m => m.Write(It.IsAny<string>(), WriteStyle.Error, true),
            Times.Never());
    }

    [Fact]
    public void Discover_Help_WritesExpected()
    {
        // Arrange
        const string command = "e2e --help";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(
            m => m.WriteStructured("Comanche.Tests", " e2e commented", null, " (Commented module.)"));
    }

    [Fact]
    public void Discover_ModuleOptIn_WritesExpected()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented --help";
        const string expected = @"
            Module: Comanche.Tests e2e commented (Commented module.)
            Sub Modules: Comanche.Tests e2e commented param-test
            Methods:
              Comanche.Tests e2e commented join-array (Join array.)
              Comanche.Tests e2e commented throw (Throws a thing.)
              Comanche.Tests e2e commented sum-array
              Comanche.Tests e2e commented pass-thru
              Comanche.Tests e2e commented next
              Comanche.Tests e2e commented sum-dicto
              Comanche.Tests e2e commented sum (Sums ints.)
        ";

        // Act
        Invoke(command, plainWriter, true);

        // Assert
        plainWriter.ShouldBe(expected);
    }

    [Fact]
    public void Discover_MethodHelpParamDefaults_WritesExpected()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented param-test change --help";
        const string expected = @"
            Module: Comanche.Tests e2e commented param-test
            Method: Comanche.Tests e2e commented param-test change
            Parameters:
            --d1 [DateTime]
            --m1 [Decimal]
            --i1 [int64? = 1]
            Returns: [DateTime]
        ";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldBe(expected);
    }

    [Fact]
    public void Discover_SerialisableReturnType_WritesExpected()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented param-test get-nums";
        const string expected = "[ 1, 2, 3 ] ";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldBe(expected);
    }

    [Theory]
    [InlineData(2L, 7200)]
    [InlineData(null, 11520)]
    public void Discover_MultipleParameters_ReturnsExpected(long? param, double expected)
    {
        // Arrange
        var d1 = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = E2ETestModule.CommentedModule.ParamTestModule.Change(d1, i1: param);
        var resultDelta = (result - d1).TotalSeconds;

        // Assert
        resultDelta.Should().Be(expected);
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("/?")]
    public void Discover_MethodHelpWithoutDocs_WritesExpected(string helpCommand)
    {
        // Arrange
        var plainWriter = new PlainWriter();
        var command = $"e2e commented sum-array {helpCommand}";
        const string expected = @"
            Module: Comanche.Tests e2e commented (Commented module.)
            Method: Comanche.Tests e2e commented sum-array
            Parameters: --n (-numbers) [int[]]
            Returns: [int]
        ";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldBe(expected);
    }

    [Fact]
    public void Discover_MethodHelpPartialDocs_WritesExpected()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented throw --help";
        const string expected = @"
            Module: Comanche.Tests e2e commented (Commented module.)
            Method: Comanche.Tests e2e commented throw (Throws a thing.)
            Parameters: --test [boolean = False] (Test.)
            Returns: [<void>]
        ";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldBe(expected);
    }

    [Fact]
    public void Discover_MethodHelpWithDocs_WritesExpected()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented join-array --help";
        const string expected = @"
            Module: Comanche.Tests e2e commented (Commented module.)
            Method: Comanche.Tests e2e commented join-array (Join array.)
            Parameters:
            --s [string[]] (The s.)
            --x [string = ""!""] (The x.)
            Returns: [string] (Val.)
        ";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldBe(expected);
    }

    [Fact]
    public void Discover_MethodHelpNoParams_WritesExpected()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented static single-mod do --help";
        const string expected = @"
Module:
Comanche.Tests e2e commented static single-mod

Method:
Comanche.Tests e2e commented static single-mod do

Returns:
[<void>]

";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldMatchVerbatim(expected);
    }

    [Fact]
    public void Discover_MethodHelpComplexParam_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented sum-dicto --help";
        const string expectedType = "[Dictionary<string, int> = null]";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(
            m => m.WriteStructured(null, "--d ", expectedType, string.Empty));
    }

    [Fact]
    public void Discover_BadRoute_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum doesnotexist";
        const string expected = "No such method.";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_BadAliasedParam_WritesExpectedError()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented sum -numbers NaN";
        const string expected = @"
Invalid Parameters:
--numbers (-n): missing
--n (-numbers): cannot convert

Note:
Run again with --help for a full parameter list.

";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldMatchVerbatim(expected);
    }

    [Fact]
    public void Discover_NoRoute_WritesExpectedError()
    {
        // Arrange
        const string command = "bloort";
        const string expected = "Invalid route.";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_DefaultArgs_WritesExpectedError()
    {
        // Arrange
        var mockWriter = new Mock<IOutputWriter>();
        const string expected = "Invalid route: --port";

        // Act
        Invoke(writer: mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_PreRouteParam_WritesExpectedError()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "--lol";
        const string expected = @"No routes found.

Module:
Comanche.Tests v1.0.0 (Test project)

Sub Modules:
Comanche.Tests discover-e2etests
Comanche.Tests e2e (Module for end to end tests.)
Comanche.Tests console-writer-tests (Tests for the class.)
Comanche.Tests plain-writer-tests (Tests for the class.)

";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldMatchVerbatim(expected);
    }

    [Fact]
    public void Discover_DefaultBoolFlagWithError_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented throw --test";
        const string expected = "1 (Parameter 'test')";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_NullableMissing_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented next";
        const string expected = "--b: missing";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_NonNullableOnlyFlag_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented static delay --ms";
        const string expected = "--ms: missing";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_BadParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum --notaparam";
        const string expected1 = "--numbers (-n): missing";
        const string expected2 = "--notaparam: unrecognised";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected1, WriteStyle.Error, true));
        mockWriter.Verify(m => m.Write(expected2, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_BadParamFormat_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum ---notaparam";
        const string expected = "Bad parameter: '---notaparam'.";
        var mockWriter = new Mock<IOutputWriter>();
        E2ETestModule.CommentedModule.StaticModule.SingleMod.Do();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_EmptyModuleDo_ReturnsExpected()
    {
        // Arrange
        const int expected = 42;

        // Act
        var actual = E2ETestModule.CommentedModule.EmptyModule.Do();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Discover_BadCall_WritesExpectedError()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented throw";
        const string expected = @"
Exception:
[ArgumentException] 2 (Parameter 'test')

Note:
Run again with --debug for more detail.

";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldMatchVerbatim(expected);
    }

    [Fact]
    public void Discover_BadCallWithDebug_WritesExpectedError()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented throw --debug";
        const string expected = @"
            Exception: [ArgumentException] 2 (Parameter 'test')
            Stack Trace: at
        ";

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldContain(expected);
    }

    [Fact]
    public void Discover_BadCallWithDebug_CallsExpectedMethods()
    {
        // Arrange
        var mockWriter = new Mock<IOutputWriter>();
        const string command = "e2e commented throw --debug";
        const string expected1 = "Stack Trace:";
        const string expected2 = "   at Comanche.Tests";
        const WriteStyle expected3 = WriteStyle.Highlight1;

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(It.Is<string?>(s => (s ?? " ").Contains(expected1)), WriteStyle.Default, true));
        mockWriter.Verify(m => m.Write(It.Is<string?>(s => (s ?? " ").StartsWith(expected2)), expected3, true));
    }

    [Fact]
    public void Discover_HiddenParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum -n 1 --other-seed 2";
        const string expected = "--other-seed: unrecognised";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_MultipleButNotSequence_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented throw --test true --test false";
        const string expected = "--test: not array";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_BadItemInList_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum -n yo";
        const string expected = "--numbers (-n): cannot convert";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_BadItemInArray_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum-array --n yo";
        const string expected = "--n (-numbers): cannot convert";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_InvalidJson_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum-dicto --d xyz";
        const string expected = "--d: cannot deserialize";
        _ = E2ETestModule.CommentedModule.SumDicto(new());
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_InvalidParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented next --b 933";
        const string expected = "--b: cannot convert";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, WriteStyle.Error, true));
    }

    [Fact]
    public void Discover_IOutputWriter_AutoInjected()
    {
        // Arrange
        const string command = "e2e commented pass-thru";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        var result = Invoke(command, mockWriter.Object);

        // Assert
        result.Should().Be(mockWriter.Object);
    }

    private static object? Invoke(
        string? command = null,
        IOutputWriter? writer = null,
        bool moduleOptIn = false)
    {
        writer ??= new Mock<IOutputWriter>().Object;
        var asm = Assembly.GetAssembly(typeof(DiscoverE2ETests));
        return Discover.Go(moduleOptIn, asm, command?.Split(' '), writer);
    }
}
