// <copyright file="RoutingExtensionsTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Extensions;

using System;
using System.Text.RegularExpressions;
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
    [InlineData("one --help -D", "one", "-D", true)]
    [InlineData("  one  TWO  --a -BB -h ", "one TWO", "--a -BB", true)]
    public void BuildRoute_ValidCommands_ReturnExpected(string command, string expRoute, string expParams, bool expHelp)
    {
        // Arrange
        var args = SplitOnSpace(command);
        var expected = new ComancheRoute(SplitOnSpace(expRoute), SplitOnSpace(expParams), expHelp);

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
    [InlineData("one --yo three", "one")]
    [InlineData("-h two", "")]
    public void BuildRoute_ParamPrecedesRoute_ThrowsExpected(string command, string expectedRoute)
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
