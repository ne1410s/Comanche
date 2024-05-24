// <copyright file="E2ERoutingTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Features.Routing;

using Comanche.Tests.Features.Console;
using E2E = TestHelper;

public class E2ERoutingTests
{
    [Fact]
    public void Routing_HiddenMiddleman_SkippedInHierarchy()
    {
        // Arrange
        const string command = "routez revealed";
        var plainWriter = new PlainWriter();
        var expected = """
            Module: testctl routez revealed
            Methods: testctl routez revealed do
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().Be(expected);
    }

    [Fact]
    public void Routing_RevealedModule_ReturnsExpected()
    {
        // Arrange
        const int expected = 42;

        // Act
        var actual = E2EUnhiddenSubRouteModule.Do();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Routing_PreRouteJunk_WritesExpectedError()
    {
        // Arrange
        const string command = "--conk routez revealed";
        const string expected = "Invalid route: --conk";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expected, true, E2E.DefaultPalette.Error, true));
    }
}
