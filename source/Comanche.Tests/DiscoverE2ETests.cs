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
    public async Task Discover_BadCall_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented throw";
        const string expected = "Unexpected error calling method 'throw'";
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
    public async Task Discover_UnsupportedParamType_WritesExpectedError()
    {
        // Arrange
        const string command = "e2e commented sumdicto --d xyz";
        const string expected = "--d: unsupported";
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        await Invoke(command, mockWriter.Object);

        // Assert
        mockWriter.Verify(m => m.WriteLine(expected, true));
    }

    private static async Task<object?> Invoke(
        string? command = null,
        IOutputWriter? writer = null,
        bool moduleOptIn = false)
    {
        writer ??= new Mock<IOutputWriter>().Object;
        var asm = Assembly.GetAssembly(typeof(DiscoverE2ETests));
        return await Discover.GoAsync(moduleOptIn, asm, command?.Split(' '), writer);
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

        /// <summary>
        /// Sums ints.
        /// </summary>
        /// <param name="numbers">Integers.</param>
        /// <param name="otherSeed">Other seed.</param>
        /// <returns>Sum plus a seed.</returns>
        [Alias("sum")]
        public int SumList(
            [Alias("n")]List<int> numbers,
            [Hidden]int otherSeed = 0) => this.Seed + otherSeed + numbers.Sum();

        public static int SumArray(int[] n) => n.Sum();

        public static int SumDicto(Dictionary<string, int> d) => d.Values.Sum();
    }
}
