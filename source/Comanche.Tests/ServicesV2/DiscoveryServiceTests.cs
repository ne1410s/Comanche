// <copyright file="DiscoveryServiceTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.ServicesV2;

using System.Reflection;
using Comanche.ServicesV2;

/// <summary>
/// Tests for the <see cref="DiscoveryService"/>.
/// </summary>
public class DiscoveryServiceTests
{
    private static readonly Assembly TestCli = Assembly.GetAssembly(typeof(CliTest.NumbersModule))!;

    [Fact]
    public void Discover_WithAssembly_FindsModules()
    {
        // Arrange
        var sut = new DiscoveryService();

        // Act
        var result = sut.Discover(TestCli);

        // Assert
    }
}
