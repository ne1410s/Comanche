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
}
