// <copyright file="DiscoverE2ETests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests;

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
        result.Should().BeEquivalentTo("hello, world");
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

        /// <summary>
        /// Joins an array of strings.
        /// </summary>
        /// <param name="s">Strings.</param>
        /// <returns>Single string.</returns>
        public static async Task<string> JoinArray(string[] s) =>
            await Task.FromResult(string.Join(", ", s));

        /// <summary>
        /// Sums a list of integers and the seed.
        /// </summary>
        /// <param name="numbers">Integers.</param>
        /// <returns>Sum plus a seed.</returns>
        [Alias("sum")]
        public int SumList([Alias("n")]List<int> numbers) => this.Seed + numbers.Sum();
    }
}
