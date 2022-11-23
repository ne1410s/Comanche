// <copyright file="MatchingExtensionsTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using Comanche.Exceptions;
using Comanche.Extensions;
using Comanche.Models;

/// <summary>
/// Tests for the <see cref="MatchingExtensions"/>.
/// </summary>
public class MatchingExtensionsTests
{
    [Fact]
    public void Match_NoRoutes_ThrowsExpected()
    {
        // Arrange
        var sut = GetSut_MultipleFlatModules();
        var route = GetRoute(string.Empty);

        // Act
        var act = () => sut.MatchMethod(route);

        // Assert
        act.Should().ThrowExactly<RouteBuilderException>()
            .Which.DeepestValidTerms.Should().BeEmpty();
    }

    [Fact]
    public void Match_NonMatchingPrimaryRoute_ThrowsExpected()
    {
        // Arrange
        var sut = GetSut_MultipleFlatModules();
        var route = GetRoute("doesnotexist");

        // Act
        var act = () => sut.MatchMethod(route);

        // Assert
        act.Should().ThrowExactly<RouteBuilderException>()
            .Which.DeepestValidTerms.Should().BeEmpty();
    }

    [Theory]
    [InlineData("mod1 mod1_m1", "mod1_m1")]
    [InlineData("mod2 mod2_m1", "mod2_m1")]
    [InlineData("mod2 mod2_m3", "mod2_m3")]
    [InlineData("Mod2 m1A", "m1A")]
    public void Match_ValidRoute_ReturnsExpected(string command, string expectedName)
    {
        // Arrange
        var sut = GetSut_MultipleFlatModules();
        var route = GetRoute(command);

        // Act
        var result = sut.MatchMethod(route);

        // Assert
        result.Name.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("mod1 m1a", "m1a")]
    [InlineData("mod1 sub3 m1d", "m1d")]
    [InlineData("mod1 sub3 sub2x1 m2e", "m2e")]
    public void Match_ValidNestedRoute_ReturnsExpected(string command, string expectedName)
    {
        // Arrange
        var sut = GetSut_SingleNestedModule();
        var route = GetRoute(command);

        // Act
        var result = sut.MatchMethod(route);

        // Assert
        result.Name.Should().Be(expectedName);
    }

    private static ComancheSession GetSut_MultipleFlatModules() => new(new()
    {
        ["mod1"] = new("mod1", null, GetMethods("mod1_m1"), new()),
        ["mod2"] = new("mod2", null, GetMethods("mod2_m1", "mod2_m2", "mod2_m3"), new()),
        ["Mod2"] = new("Mod2", null, GetMethods("m1A"), new()),
    });

    private static ComancheSession GetSut_SingleNestedModule() => new(new()
    {
        ["mod1"] = new("mod1", null, GetMethods("m1a", "m2a"), new()
        {
            ["sub1"] = new("sub1", null, GetMethods("m1b", "m2b"), new()),
            ["sub2"] = new("sub2", null, GetMethods("m1c", "m2c", "m3c", "m4c"), new()),
            ["sub3"] = new("sub3", null, GetMethods("m1d"), new()
            {
                ["sub2x1"] = new("sub2x1", null, GetMethods("m1e", "m2e"), new()),
            }),
        }),
    });

    private static Dictionary<string, ComancheMethod> GetMethods(params string[] names)
        => names.ToDictionary(
            n => n,
            n => new ComancheMethod(n, null, null, typeof(void), null!, null!, null!));

    private static ComancheRoute GetRoute(string termsInput, bool isHelp = false)
        => new(SplitOnSpace(termsInput), new(), isHelp);

    private static string[] SplitOnSpace(string input)
        => input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
}
