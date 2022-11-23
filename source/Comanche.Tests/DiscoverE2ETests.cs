// <copyright file="DiscoverE2ETests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests;

using System.Reflection;
using System.Threading.Tasks;
using Comanche.Attributes;
using Comanche.Services;

[Module("e2e")]
public class DiscoverE2ETests
{
    [Fact]
    public async Task Discover_NoOptIn_FindsClasses()
    {
        // Arrange
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        var result = await Invoke(mockWriter.Object, moduleOptIn: false);

        // Assert
        mockWriter.Verify(m => m.WriteLine("MODULE: e2e", false), Times.Once());
    }

    [Fact]
    public async Task Discover_SubModule_FindsClasses()
    {
        // Arrange
        var mockWriter = new Mock<IOutputWriter>();

        // Act
        var result = await Invoke(mockWriter.Object, "e2e");

        // Assert
        mockWriter.Verify(m => m.WriteLine("MODULE: commented (Commented module.)", false), Times.Once());
    }

    private static async Task<object?> Invoke(
        IOutputWriter? writer = null,
        string? command = null,
        bool moduleOptIn = false)
    {
        var asm = Assembly.GetAssembly(typeof(DiscoverE2ETests));
        writer ??= new Mock<IOutputWriter>().Object;
        return await Discover.GoAsync(moduleOptIn, asm, command?.Split(' '), writer);
    }

    /// <summary>
    /// Commented module.
    /// </summary>
    public sealed class CommentedModule
    {
        public static int Add1(int integer) => integer + 1;
    }
}
