// <copyright file="DiscoverE2ETests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Comanche.Attributes;
using Comanche.Services;

[Module("e2e")]
public class DiscoverE2ETests
{
    [Fact]
    public async Task Discover_AltParams_WritesExpected()
    {
        // Act
        var result = await Discover.GoAsync(true);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Discover_StringArray_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented joinarray --s hello --s world";

        // Act
        var result = await Invoke(command);

        // Assert
        result.Should().BeEquivalentTo("hello, world!");
    }

    [Fact]
    public async Task Discover_IntArray_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented sumarray --n 3 --n 4 --n 1";

        // Act
        var result = await Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(8);
    }

    [Fact]
    public async Task Discover_IntList_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented sum -n 3 -n 4 -n 1";

        // Act
        var result = await Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(20);
    }

    [Fact]
    public async Task Discover_NullableWithValue_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented next --b 110";

        // Act
        var result = await Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(111);
    }

    [Fact]
    public async Task Discover_NullableWithFlag_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented next --b";

        // Act
        var result = await Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(256);
    }

    [Fact]
    public async Task Discover_GoodJson_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented sumdicto --d { \"a\": 1, \"b\": 2 }";

        // Act
        var result = await Invoke(command);

        // Assert
        result.Should().BeEquivalentTo(3);
    }

    [Fact]
    public async Task Discover_ReturnBareTask_ReturnsExpected()
    {
        // Arrange
        const string command = "e2e commented static delay --ms 1";

        // Act
        var result = await Invoke(command);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Discover_NoRoute_WritesExpected()
    {
        // Arrange
        const string command = "bloort";
        const string expected = "MODULE: e2e";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, false));
    }

    [Fact]
    public async Task Discover_MethodHelp_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented sum --help";
        const string expected = "  --numbers (-n) [List`1] - Integers.";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, false));
    }

    [Fact]
    public async Task Discover_MethodHelpWithString_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented joinarray --help";
        const string expected = "  --x [String = !]";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, false));
    }

    [Fact]
    public async Task Discover_BadRoute_WritesExpected()
    {
        // Arrange
        const string command = "e2e commented sum doesnotexist";
        const string expected = "METHOD: sum (Sums ints.)";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, false));
    }

    [Fact]
    public async Task Discover_PreRouteParam_WritesExpectedError()
    {
        // Arrange
        const string command = "--lol";
        const string expected = "Exception of type 'Comanche.Exceptions.RouteBuilderException' was thrown.";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_DefaultBoolFlagWithError_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented throw --test";
        const string expected = "Error calling 'throw': 1 (Parameter 'test')";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_NullableMissing_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented next";
        const string expected = "--b: missing";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_NonNullableOnlyFlag_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented static delay --ms";
        const string expected = "--ms: missing";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_BadParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum --notaparam";
        const string expected = "--numbers (-n): missing";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_BadParamFormat_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum ---notaparam";
        const string expected = "Exception of type 'Comanche.Exceptions.RouteBuilderException' was thrown.";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_BadCall_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented throw";
        const string expected = "Error calling 'throw': 2 (Parameter 'test')";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_HiddenParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum -n 1 --otherSeed 2";
        const string expected = "--otherSeed: unrecognised";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_MultipleButNotSequence_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented throw --test true --test false";
        const string expected = "--test: not array";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_BadItemInList_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sum -n yo";
        const string expected = "--numbers (-n): cannot convert";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_BadItemInArray_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sumarray --n yo";
        const string expected = "--n: cannot convert";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_InvalidJson_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sumdicto --d xyz";
        const string expected = "--d: cannot deserialize";
        _ = CommentedModule.SumDicto(new());
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    [Fact]
    public async Task Discover_InvalidParam_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented next --b 933";
        const string expected = "--b: cannot convert";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    private static async Task<object?> Invoke(
        string command,
        IOutputWriter? writer = null,
        bool moduleOptIn = false)
    {
        writer ??= new Mock<IOutputWriter>().Object;
        var asm = Assembly.GetAssembly(typeof(DiscoverE2ETests));
        return await Discover.GoAsync(moduleOptIn, asm, command.Split(' '), writer);
    }

    /// <summary>
    /// Commented module.
    /// </summary>
    public sealed class CommentedModule
    {
        private int Seed { get; } = 12;

        public static async Task<string> JoinArray(string[] s, string x = "!") =>
            await Task.FromResult(string.Join(", ", s) + x);

        public static void Throw(bool test = false) =>
            throw new ArgumentException(test ? "1" : "2", nameof(test));

        public static int SumArray(int[] n) => n.Sum();

        public static short Next([Alias(null!)]byte? b) => (short)((b ?? byte.MaxValue) + 1);

        public static int SumDicto(Dictionary<string, int> d) => d.Values.Sum();

        /// <summary>
        /// Sums ints.
        /// </summary>
        /// <param name="numbers">Integers.</param>
        /// <param name="otherSeed">Other seed.</param>
        /// <returns>Sum plus a seed.</returns>
        [Alias("sum")]
        public int SumList(
            [Alias("n")] List<int> numbers,
            [Hidden] int otherSeed = 0) => this.Seed + otherSeed + numbers.Sum();

        [Module(null!)]
        public static class StaticModule
        {
            public static async Task Delay(int ms) => await Task.Delay(ms);

            public static class EmptyMod { }
        }
    }
}
