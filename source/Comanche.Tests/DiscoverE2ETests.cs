﻿// <copyright file="DiscoverE2ETests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests;

using System.Reflection;
using Comanche.Services;

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
        mockWriter.Verify(m => m.WriteLine(expected, false));
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
            m => m.WriteLine(It.IsAny<string>(), true),
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
            m => m.WriteLine(It.IsAny<string>(), true),
            Times.Never());
    }

    [Fact]
    public void Discover_Help_WritesExpected()
    {
        // Arrange
        const string command = "e2e --help";
        const string expected = "MODULE: commented (Commented module.)";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, false));
    }

    [Fact]
    public void Discover_ModuleOptIn_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented --help";
        const string unexpected = "MODULE: static";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object, true);

        // Assert
        mockWriter.Verify(m => m.WriteLine(unexpected, It.IsAny<bool>()), Times.Never());
    }

    [Fact]
    public void Discover_OptedInEmptyMod_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented --help";
        const string unexpected = "MODULE: empty";
        var mockWriter = new Mock<IOutputWriter>();
        _ = E2ETestModule.CommentedModule.EmptyModule.Do();

        // Act
        Invoke(command, mockWriter.Object, true);

        // Assert
        mockWriter.Verify(m => m.WriteLine(unexpected, It.IsAny<bool>()), Times.Never());
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("/?")]
    public void Discover_MethodHelpWithoutDocs_WritesExpected(string helpCommand)
    {
        // Arrange
        var command = $"e2e commented sum-array {helpCommand}";
        const string expected1 = "- Method: sum-array";
        const string expected2 = "- Parameters:";
        const string expected3 = "  --n (-numbers) [Int32[]]";
        const string expected4 = "- Returns: [Int32]";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected1, false));
        mockWriter.Verify(m => m.WriteLine(expected2, false));
        mockWriter.Verify(m => m.WriteLine(expected3, false));
        mockWriter.Verify(m => m.WriteLine(expected4, false));
    }

    [Fact]
    public void Discover_MethodHelpPartialDocs_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented throw --help";
        const string expected1 = "- Method: throw";
        const string expected2 = "- Summary: Throws a thing.";
        const string expected3 = "- Returns: [Void]";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected1, false));
        mockWriter.Verify(m => m.WriteLine(expected2, false));
        mockWriter.Verify(m => m.WriteLine(expected3, false));
    }

    [Fact]
    public void Discover_MethodHelpWithDocs_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented join-array --help";
        const string expected1 = "- Method: join-array";
        const string expected2 = "- Summary: Join array.";
        const string expected3 = "- Parameters:";
        const string expected4 = "  --x [String = !] - The x.";
        const string expected5 = "- Returns: [Task`1] Val.";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected1, false));
        mockWriter.Verify(m => m.WriteLine(expected2, false));
        mockWriter.Verify(m => m.WriteLine(expected3, false));
        mockWriter.Verify(m => m.WriteLine(expected4, false));
        mockWriter.Verify(m => m.WriteLine(expected5, false));
    }

    [Fact]
    public void Discover_MethodHelpNoParams_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented static single-mod do --help";
        const string unexpected = "- Parameters:";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(unexpected, It.IsAny<bool>()), Times.Never());
    }

    [Fact]
    public void Discover_MethodHelpComplexParam_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented sum-dicto --help";
        const string expected = "  --d [Dictionary`2]";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, false));
    }

    [Fact]
    public void Discover_BadRoute_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum doesnotexist";
        const string expected1 = "No such method.";
        const string expected2 = "METHOD: sum (Sums ints.)";
        const string expected3 = "METHOD: next";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected1, true));
        mockWriter.Verify(m => m.WriteLine(expected2, false));
        mockWriter.Verify(m => m.WriteLine(expected3, false));
    }

    [Fact]
    public void Discover_BadAliasedParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum -numbers NaN";
        const string expected1 = "Invalid parameters";
        const string expected2 = "--n (-numbers): cannot convert";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected1, true));
        mockWriter.Verify(m => m.WriteLine(expected2, true));
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
        mockWriter.Verify(m => m.WriteLine(expected, true));
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
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public void Discover_PreRouteParam_WritesExpectedError()
    {
        // Arrange
        const string command = "--lol";
        const string expected = "No routes found.";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public void Discover_DefaultBoolFlagWithError_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented throw --test";
        const string expected = "Error calling 'throw': 1 (Parameter 'test')";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
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
        mockWriter.Verify(m => m.WriteLine(expected, true));
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
        mockWriter.Verify(m => m.WriteLine(expected, true));
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
        mockWriter.Verify(m => m.WriteLine(expected1, true));
        mockWriter.Verify(m => m.WriteLine(expected2, true));
    }

    [Fact]
    public void Discover_BadParamFormat_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum ---notaparam";
        const string expected1 = "Bad parameter: '---notaparam'.";
        const string expected2 = "MODULE: static";
        var mockWriter = new Mock<IOutputWriter>();
        E2ETestModule.CommentedModule.StaticModule.SingleMod.Do();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected1, true));
        mockWriter.Verify(m => m.WriteLine(expected2, false));
    }

    [Fact]
    public void Discover_BadCall_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented throw";
        const string expected = "Error calling 'throw': 2 (Parameter 'test')";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public void Discover_HiddenParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum -n 1 --otherSeed 2";
        const string expected = "--otherSeed: unrecognised";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
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
        mockWriter.Verify(m => m.WriteLine(expected, true));
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
        mockWriter.Verify(m => m.WriteLine(expected, true));
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
        mockWriter.Verify(m => m.WriteLine(expected, true));
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
        mockWriter.Verify(m => m.WriteLine(expected, true));
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
        mockWriter.Verify(m => m.WriteLine(expected, true));
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
