﻿// <copyright file="DiscoverE2ETests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests;

using System;
using System.Reflection;
using Comanche.Models;
using Comanche.Services;
using Comanche.Tests.Services;
using Microsoft.Extensions.DependencyInjection;

public class DiscoverE2ETests
{
    private static readonly ComanchePalette StandardPalette = new();

    [Fact]
    public void Discover_AltParams_ReturnsExpected()
    {
        // Act
        var result = Discover.Go();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Discover_WithoutIConsole_AddsDefaultConsole()
    {
        // Arrange
        var stubServices = new ServiceCollection();

        // Act
        Discover.Go(stubServices);

        // Assert
        stubServices.Should().Contain(d => d.ImplementationType == typeof(ConsoleWriter));
    }

    [Fact]
    public void Discover_WithIConsole_DoesNotAddDefaultConsole()
    {
        // Arrange
        var stubServices = new ServiceCollection();
        stubServices.AddTransient(_ => GetMockConsole().Object);

        // Act
        Discover.Go(stubServices);

        // Assert
        stubServices.Should().NotContain(d => d.ImplementationType == typeof(ConsoleWriter));
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
        var mockWriter = GetMockConsole();

        // Act
        _ = Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, null, false));
    }

    [Fact]
    public void Discover_EmptyCommand_DoesNotWriteError()
    {
        // Arrange
        const string command = "";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(
            m => m.Write(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<ConsoleColor>(), true),
            Times.Never());
    }

    [Theory]
    [InlineData("e2e")]
    [InlineData("e2e commented static single-mod")]
    public void Discover_BareModule_DoesNotWriteError(string command)
    {
        // Arrange
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(
            m => m.Write(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<ConsoleColor>(), true),
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
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(
            m => m.Write(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<ConsoleColor>(), true),
            Times.Never());
    }

    [Fact]
    public void Discover_Help_WritesExpected()
    {
        // Arrange
        const string command = "e2e --help";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write("Comanche.Tests", false, StandardPalette.Primary, false));
        mockWriter.Verify(m => m.Write(" e2e commented", false, null, false));
        mockWriter.Verify(m => m.Write(" (Commented module.)", false, StandardPalette.Tertiary, false));
    }

    [Fact]
    public void Discover_ModuleOptIn_WritesExpected()
    {
        // Arrange
        var plainWriter = new PlainWriter();
        const string command = "e2e commented --help";
        const string expected = @"
            Module: Comanche.Tests e2e commented (Commented module.)
            Sub Modules:
              Comanche.Tests e2e commented static
              Comanche.Tests e2e commented param-test
              Comanche.Tests e2e commented guidz
              Comanche.Tests e2e commented enumz
              Comanche.Tests e2e commented di
              Comanche.Tests e2e commented missing-di
            Methods:
              Comanche.Tests e2e commented join-array (Join array.)
              Comanche.Tests e2e commented throw (Throws a thing.)
              Comanche.Tests e2e commented sum-array
              Comanche.Tests e2e commented pass-thru
              Comanche.Tests e2e commented pass-thru-hidden
              Comanche.Tests e2e commented next
              Comanche.Tests e2e commented sum-dicto
              Comanche.Tests e2e commented sum (Sums ints.)
        ";

        // Act
        Invoke(command, plainWriter);

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
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expectedType, false, StandardPalette.Secondary, false));
    }

    [Fact]
    public void Discover_BadRoute_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum doesnotexist";
        const string expected = "No such method.";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
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
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_DefaultArgs_WritesExpectedError()
    {
        // Arrange
        var mockWriter = GetMockConsole();
        const string expected = "Invalid route: --port";

        // Act
        Invoke(writer: mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
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
Comanche.Tests e2e (Module for end to end tests.)

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
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_NullableMissing_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented next";
        const string expected = "--b: missing";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_NonNullableOnlyFlag_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented static delay --ms";
        const string expected = "--ms: missing";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_BadParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum --notaparam";
        const string expected1 = "--numbers (-n): missing";
        const string expected2 = "--notaparam: unrecognised";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected1, true, StandardPalette.Error, true));
        mockWriter.Verify(m => m.Write(expected2, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_BadParamFormat_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum ---notaparam";
        const string expected = "Bad parameter: '---notaparam'.";
        var mockWriter = GetMockConsole();
        E2ETestModule.CommentedModule.StaticModule.SingleMod.Do();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
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
        var mockWriter = GetMockConsole();
        const string command = "e2e commented throw --debug";
        const string expected1 = "Stack Trace:";
        const string expected2 = "   at Comanche.Tests";
        var expected4 = StandardPalette.Primary;

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(It.Is<string?>(s => (s ?? " ").Contains(expected1)), true, null, false));
        mockWriter.Verify(m => m.Write(It.Is<string?>(s => (s ?? " ").StartsWith(expected2)), true, expected4, false));
    }

    [Fact]
    public void Discover_StacklessExceptionWithDebug_WritesExpected()
    {
        // Arrange
        var mockWriter = GetMockConsole();
        var primary = mockWriter.Object.Palette.Primary;
        const string command = "e2e commented static can-throw --debug";
        mockWriter.Setup(m => m.Write("throw bro", false, null, false)).Throws(new StacklessException());

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(string.Empty, true, primary, false));
    }

    [Fact]
    public void Discover_HiddenParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum -n 1 --other-seed 2";
        const string expected = "--other-seed: unrecognised";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_MultipleButNotSequence_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented throw --test true --test false";
        const string expected = "--test: not array";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_BadItemInList_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum -n yo";
        const string expected = "--numbers (-n): cannot convert";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_BadItemInArray_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum-array --n yo";
        const string expected = "--n (-numbers): cannot convert";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_InvalidJson_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum-dicto --d xyz";
        const string expected = "--d: cannot deserialize";
        _ = E2ETestModule.CommentedModule.SumDicto([]);
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_InvalidParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented next --b 933";
        const string expected = "--b: cannot convert";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_GuidIn_ReturnsValue()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var command = $"e2e commented guidz stringify-guid --id {expected}";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, null, false));
    }

    [Fact]
    public void Discover_OptionalGuidIn_ReturnsValue()
    {
        // Arrange
        var expected = Guid.NewGuid().ToString();
        var command = $"e2e commented guidz stringify-optional-guid --id {expected}";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, null, false));
    }

    [Fact]
    public void Discover_MissingOptionalGuidIn_ReturnsNull()
    {
        // Arrange
        const string command = "e2e commented guidz stringify-optional-guid";
        var mockWriter = GetMockConsole();

        // Act
        var result = Invoke(command, mockWriter.Object);

        // Assert
        result.Should().BeNull();
        mockWriter.Verify(m => m.Write(string.Empty, true, null, false));
    }

    [Fact]
    public void Discover_NonGuidIn_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented guidz stringify-guid --id h3ll0_Gu1dz";
        const string expected = "--id: cannot parse guid";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Theory]
    [InlineData(nameof(DayOfWeek.Wednesday))]
    [InlineData("weDnesDAY")]
    [InlineData((int)DayOfWeek.Wednesday)]
    public void Discover_EnumSet_AcceptsNumberOrCIText(object paramLiteral)
    {
        // Arrange
        const int expected = (int)DayOfWeek.Wednesday;
        var command = $"e2e commented enumz set --day {paramLiteral}";

        // Act
        var result = Invoke(command);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Discover_EnumSetInvalid_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented enumz set --day Wodinsday";
        const string expected = "--day: not in enum";
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.Write(expected, true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_EnumGetDirect_ReturnsEnumAndWritesText()
    {
        // Arrange
        const string command = "e2e commented enumz get-direct";
        const DayOfWeek expectedResult = DayOfWeek.Friday;
        const string expectedText = nameof(DayOfWeek.Friday);
        var mockWriter = GetMockConsole();

        // Act
        var result = Invoke(command, mockWriter.Object);

        // Assert
        result.Should().Be(expectedResult);
        mockWriter.Verify(m => m.Write(expectedText, true, null, false));
    }

    [Fact]
    public void Discover_EnumGetNested_WritesEnumText()
    {
        // Arrange
        const string command = "e2e commented enumz get-nested";
        const string expectedText = "{ \"day\": \"Friday\" }";
        var plainWriter = new PlainWriter();

        // Act
        Invoke(command, plainWriter);

        // Assert
        plainWriter.ShouldContain(expectedText);
    }

    [Fact]
    public void Discover_EnvConfig_ReturnsExpected()
    {
        // Arrange
        const string key = "TEST";
        var expected = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable(key, expected);
        const string command = "e2e commented enumz get-config --key " + key;

        // Act
        var actual = Invoke(command);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, "dev")]
    [InlineData("Development", "dev")]
    [InlineData("NonExisting", null)]
    [InlineData("Custom", "custom")]
    public void Discover_JsonConfigDifferentEnvironments_ReturnsExpected(string? environmentName, string? expected)
    {
        // Arrange
        Environment.SetEnvironmentVariable(Discover.EnvironmentKey, environmentName);
        const string command = "e2e commented enumz get-config --key ConfigName";

        // Act
        var actual = Invoke(command);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Discover_IOutputWriterNotHidden_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented pass-thru";
        _ = E2ETestModule.CommentedModule.PassThru(null!);
        var mockWriter = GetMockConsole();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(
            m => m.Write("--writer: injected parameter must be hidden", true, StandardPalette.Error, true));
    }

    [Fact]
    public void Discover_IOutputWriterHidden_ReturnsWriter()
    {
        // Arrange
        const string command = "e2e commented pass-thru-hidden";
        var mockWriter = GetMockConsole();

        // Act
        var result = Invoke(command, mockWriter.Object);

        // Assert
        result.Should().Be(mockWriter.Object);
    }

    [Fact]
    public void Discover_MissingDi_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented missing-di get";
        var mockWriter = GetMockConsole();

        // Act
        var act = () => Invoke(command, mockWriter.Object);

        // Assert
        act.Should().Throw<NullReferenceException>();
        new E2ETestModule.CommentedModule.MissingDIModule(null!).Get();
    }

    [Fact]
    public void Discover_ModuleDi_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented di get-name";
        const string expected = "dev";
        Environment.SetEnvironmentVariable(Discover.EnvironmentKey, null);

        // Act
        var result = Invoke(command);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Discover_ModuleDiParamless_ReturnsNull()
    {
        // Arrange
        var module = new E2ETestModule.CommentedModule.DIModule();

        // Act
        var result = module.GetName();

        // Assert
        result.Should().BeNull();
    }

    private static object? Invoke(
        string? command = null,
        IConsole? writer = null)
    {
        writer ??= GetMockConsole().Object;
        var services = new ServiceCollection();
        services.AddSingleton(writer);

        var asm = Assembly.GetAssembly(typeof(DiscoverE2ETests));
        return Discover.Go(services, command?.Split(' '), asm);
    }

    private static Mock<IConsole> GetMockConsole()
    {
        var mockConsole = new Mock<IConsole>();
        mockConsole.Setup(m => m.Palette).Returns(new ComanchePalette());
        return mockConsole;
    }

    private sealed class StacklessException : Exception
    {
        public override string? StackTrace => null;
    }
}
