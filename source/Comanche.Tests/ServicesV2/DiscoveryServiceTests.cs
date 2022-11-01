// <copyright file="DiscoveryServiceTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.ServicesV2;

using System.Reflection;
using Comanche.Extensions;
using Comanche.Models;

/// <summary>
/// Tests for the <see cref="DiscoveryExtensions"/>.
/// </summary>
public class DiscoveryServiceTests
{
    private static readonly Assembly TestCli = Assembly.GetAssembly(typeof(CliTest.NumbersModule))!;

    [Fact]
    public void Ctor_WithAssembly_FindsModules()
    {
        // Arrange
        var sut = TestCli.Discover();

        // Act
        var result = sut.Route(new[] { "--help" });

        // Assert

    }
}
