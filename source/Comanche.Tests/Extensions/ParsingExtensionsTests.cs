// <copyright file="ParsingExtensionsTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Extensions;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Comanche.Extensions;
using Comanche.Models;

/// <summary>
/// Tests for the <see cref="ParsingExtensions"/>.
/// </summary>
public class ParsingExtensionsTests
{
    [Fact]
    public void ParseMap_WithInt_ParsesOk()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInt", null, null, typeof(int), false, false, null),
        };
        var input = new Dictionary<string, List<string>>()
        {
            ["--myInt"] = new(new[] { "12" }),
        };
        var expected = new object[] { 12 };

        // Act
        var result = sut.ParseMap(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
