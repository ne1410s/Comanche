// <copyright file="RoutingExtensionsTests.cs" company="ne1410s">
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
/// Tests for the <see cref="RoutingExtensions"/>.
/// </summary>
public class RoutingExtensionsTests
{
    [Theory]
    [InlineData("func", "func", "", false)]
    [InlineData("a b  c", "a b c", "", false)]
    [InlineData("one -D", "one", "-D", false)]
    [InlineData("  one  TWO  --a -BB ", "one TWO", "--a -BB", false)]
    [InlineData("func -h", "func", "", true)]
    [InlineData("a b  c  /?", "a b c", "", true)]
    [InlineData("a b  c -x /? --y", "a b c", "-x --y", true)]
    [InlineData("one --help -D", "one", "-D", true)]
    [InlineData("  one  TWO  --a -BB -h ", "one TWO", "--a -BB", true)]
    public void BuildRoute_ValidFlags_ReturnsExpected(string command, string expRoute, string expParams, bool expHelp)
    {
        // Arrange
        var args = SplitOnSpace(command);
        var expectParams = SplitOnSpace(expParams).ToDictionary(f => f, _ => new List<string>());
        var expected = new ComancheRoute(SplitOnSpace(expRoute), expectParams, expHelp);

        // Act
        var result = args.BuildRoute();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void BuildRoute_SplitArrayParams_ReturnsExpected()
    {
        const string command = "func -t table1 -t table2 --force -t table3";
        var args = SplitOnSpace(command);
        var expectRoutes = new string[] { "func" };
        var expectParams = new Dictionary<string, List<string>>
        {
            ["-t"] = new(new[] { "table1", "table2", "table3" }),
            ["--force"] = new(),
        };

        var expected = new ComancheRoute(expectRoutes, expectParams, false);

        // Act
        var result = args.BuildRoute();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void BuildRoute_WhiteSpaceParams_ReturnsExpected()
    {
        const string command = "func -t table1 table2 table3 --force";
        var args = SplitOnSpace(command);
        var expectRoutes = new string[] { "func" };
        var expectParams = new Dictionary<string, List<string>>
        {
            ["-t"] = new(new[] { "table1", "table2", "table3" }),
            ["--force"] = new(),
        };

        var expected = new ComancheRoute(expectRoutes, expectParams, false);

        // Act
        var result = args.BuildRoute();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void BuildRoute_LiteralSpacesParam_ReturnsExpected()
    {
        const string command = "func -t \"table1 table2 table3\" --force";
        var args = SplitOnSpace(command);
        var expectRoutes = new string[] { "func" };
        var expectParams = new Dictionary<string, List<string>>
        {
            ["-t"] = new(new[] { "table1 table2 table3" }),
            ["--force"] = new(),
        };

        var expected = new ComancheRoute(expectRoutes, expectParams, false);

        // Act
        var result = args.BuildRoute();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("-h")]
    [InlineData("99")]
    [InlineData("_lol")]
    [InlineData("--x -y -Z")]
    public void BuildRoute_NoRoutes_ThrowsExpected(string command)
    {
        // Arrange
        var args = SplitOnSpace(command);

        // Act
        var act = () => args.BuildRoute();

        // Assert
        act.Should().ThrowExactly<RouteBuilderException>()
            .Which.DeepestValidTerms.Should().BeEmpty();
    }

    [Theory]
    [InlineData("9 one")]
    [InlineData("-h one")]
    [InlineData("-x one")]
    [InlineData("/p one")]
    public void BuildRoute_RouteNotFirstItem_ThrowsExpected(string command)
    {
        // Arrange
        var args = SplitOnSpace(command);

        // Act
        var act = () => args.BuildRoute();

        // Assert
        act.Should().ThrowExactly<RouteBuilderException>()
            .Which.DeepestValidTerms.Should().BeEmpty();
    }

    [Theory]
    [InlineData("one two /bb three", "one two")]
    [InlineData("one ---yo three", "one")]
    [InlineData("one 222", "one")]
    public void BuildRoute_BadParamPrefix_ThrowsExpected(string command, string expectedRoute)
    {
        // Arrange
        var args = SplitOnSpace(command);
        var expected = SplitOnSpace(expectedRoute);

        // Act
        var act = () => args.BuildRoute();

        // Assert
        act.Should().ThrowExactly<RouteBuilderException>()
            .Which.DeepestValidTerms.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("one two 9z", "one two")]
    [InlineData("one _ two", "one")]
    [InlineData("one two ~h zz", "one two")]
    [InlineData("one two _three", "one two")]
    [InlineData("t3 -a --b ---c", "t3")]
    [InlineData("9z one", "")]
    public void BuildRoute_TermNotRouteOrParamOrHelp_ThrowsExpected(string command, string expectedRoute)
    {
        // Arrange
        var args = SplitOnSpace(command);
        var expected = SplitOnSpace(expectedRoute);

        // Act
        var act = () => args.BuildRoute();

        // Assert
        act.Should().ThrowExactly<RouteBuilderException>()
            .Which.DeepestValidTerms.Should().BeEquivalentTo(expected);
    }

    private static string[] SplitOnSpace(string input)
        => input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
}
