// <copyright file="ParsingExtensionsTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Extensions;

using System;
using System.Collections.Generic;
using Comanche.Exceptions;
using Comanche.Extensions;
using Comanche.Models;

/// <summary>
/// Tests for the <see cref="ParsingExtensions"/>.
/// </summary>
public class ParsingExtensionsTests
{
    [Fact]
    public void ParseMap_WithNamed_ParsesOk()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInt", null, null, typeof(int), false, false, null),
        };
        var input = new Dictionary<string, List<string>>()
        {
            ["--myInt"] = new() { "12" },
        };
        var expected = new object[] { 12 };

        // Act
        var result = sut.ParseMap(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ParseMap_WithAliased_ParsesOk()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInt", null, "int", typeof(int), false, false, null),
        };
        var input = new Dictionary<string, List<string>>()
        {
            ["-int"] = new() { "12" },
        };
        var expected = new object[] { 12 };

        // Act
        var result = sut.ParseMap(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

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
            ["--myInt"] = new() { "12" },
        };
        var expected = new object[] { 12 };

        // Act
        var result = sut.ParseMap(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("true", true)]
    [InlineData("False", false)]
    public void ParseMap_WithBoolPresence_ParsesOk(string inputVal, bool expectedVal)
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myBool", null, null, typeof(bool), false, false, null),
        };
        var input = new Dictionary<string, List<string>>()
        {
            ["--myBool"] = new() { inputVal },
        };
        var expected = new object[] { expectedVal };

        // Act
        var result = sut.ParseMap(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ParseMap_WithArray_ParsesOk()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInt", null, null, typeof(int[]), false, false, null),
        };
        var input = new Dictionary<string, List<string>>()
        {
            ["--myInt"] = new() { "12", "3" },
        };
        var expected = new object[] { new[] { 12, 3 } };

        // Act
        var result = sut.ParseMap(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ParseMap_WithList_ParsesOk()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInt", null, null, typeof(List<int>), false, false, null),
        };
        var input = new Dictionary<string, List<string>>()
        {
            ["--myInt"] = new() { "12", "3" },
        };
        var expected = new object[] { new List<int> { 12, 3 } };

        // Act
        var result = sut.ParseMap(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ParseMap_WithDefault_ParsesOk()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myStr", null, null, typeof(string), false, true, "defVal"),
        };
        var input = new Dictionary<string, List<string>>();
        var expected = new object[] { "defVal" };

        // Act
        var result = sut.ParseMap(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ParseMap_WithHidden_ParsesOk()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInt", null, null, typeof(int), true, true, 9),
        };
        var input = new Dictionary<string, List<string>>();
        var expected = new object[] { 9 };

        // Act
        var result = sut.ParseMap(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ParseMap_WithUnknownInput_ThrowsExpected()
    {
        // Arrange
        var sut = Array.Empty<ComancheParam>();
        var input = new Dictionary<string, List<string>>
        {
            ["--notThere"] = new() { "true" },
        };
        var expectedErrors = new Dictionary<string, string>
        {
            ["--notThere"] = "unrecognised",
        };

        // Act
        var act = () => sut.ParseMap(input);

        // Assert
        act.Should().ThrowExactly<ParamBuilderException>()
            .WithMessage("Invalid parameters found.")
            .Which.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void ParseMap_ProvidedButHidden_ThrowsExpected()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInt", null, null, typeof(int), true, true, 9),
        };
        var input = new Dictionary<string, List<string>>
        {
            ["--myInt"] = new() { "4" },
        };
        var expectedErrors = new Dictionary<string, string>
        {
            ["--myInt"] = "unrecognised",
        };

        // Act
        var act = () => sut.ParseMap(input);

        // Assert
        act.Should().ThrowExactly<ParamBuilderException>()
            .WithMessage("Invalid parameters found.")
            .Which.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void ParseMap_MultipleValsButNotArray_ThrowsExpected()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInt", null, null, typeof(int), false, false, null),
        };
        var input = new Dictionary<string, List<string>>
        {
            ["--myInt"] = new() { "4", "5" },
        };
        var expectedErrors = new Dictionary<string, string>
        {
            ["--myInt"] = "not array",
        };

        // Act
        var act = () => sut.ParseMap(input);

        // Assert
        act.Should().ThrowExactly<ParamBuilderException>()
            .WithMessage("Invalid parameters found.")
            .Which.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void ParseMap_MissingAndNoDefault_ThrowsExpected()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInt", null, null, typeof(int), false, false, null),
        };
        var input = new Dictionary<string, List<string>>();
        var expectedErrors = new Dictionary<string, string>
        {
            ["--myInt"] = "missing",
        };

        // Act
        var act = () => sut.ParseMap(input);

        // Assert
        act.Should().ThrowExactly<ParamBuilderException>()
            .WithMessage("Invalid parameters found.")
            .Which.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void ParseMap_NameAliasDuplicate_ThrowsExpected()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInt", null, "int", typeof(int), false, false, null),
        };
        var input = new Dictionary<string, List<string>>
        {
            ["--myInt"] = new() { "4" },
            ["-int"] = new() { "5" },
        };
        var expectedErrors = new Dictionary<string, string>
        {
            ["--myInt (-int)"] = "duplicate",
        };

        // Act
        var act = () => sut.ParseMap(input);

        // Assert
        act.Should().ThrowExactly<ParamBuilderException>()
            .WithMessage("Invalid parameters found.")
            .Which.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void ParseMap_MultiDimensionalArrayParam_ThrowsExpected()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myInts", null, null, typeof(int[][]), false, false, null),
        };
        var input = new Dictionary<string, List<string>>
        {
            ["--myInts"] = new() { "3" },
        };
        var expectedErrors = new Dictionary<string, string>
        {
            ["--myInts"] = "unsupported",
        };

        // Act
        var act = () => sut.ParseMap(input);

        // Assert
        act.Should().ThrowExactly<ParamBuilderException>()
            .WithMessage("Invalid parameters found.")
            .Which.Errors.Should().BeEquivalentTo(expectedErrors);
    }


    [Fact]
    public void ParseMap_DictionaryParam_ThrowsExpected()
    {
        // Arrange
        var sut = new[]
        {
            new ComancheParam("myMap", null, null, typeof(Dictionary<string, int>), false, false, null),
        };
        var input = new Dictionary<string, List<string>>
        {
            ["--myMap"] = new() { "3" },
        };
        var expectedErrors = new Dictionary<string, string>
        {
            ["--myMap"] = "unsupported",
        };

        // Act
        var act = () => sut.ParseMap(input);

        // Assert
        act.Should().ThrowExactly<ParamBuilderException>()
            .WithMessage("Invalid parameters found.")
            .Which.Errors.Should().BeEquivalentTo(expectedErrors);
    }
}
