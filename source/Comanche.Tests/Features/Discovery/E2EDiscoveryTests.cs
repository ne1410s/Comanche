// <copyright file="E2EDiscoveryTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Features.Discovery;

using System;
using Comanche.Tests.Features.Console;
using E2E = TestHelper;

public class E2EDiscoveryTests
{
    [Theory]
    [InlineData("")]
    [InlineData("--help")]
    public void Discovery_RootHelpOrEmptyCommand_WritesExpected(string command)
    {
        // Arrange
        var plainWriter = new PlainWriter();
        var expected = """
            Module: testctl v1.0.0-testing123 (Test project)
            Sub Modules:
              testctl disco (Discovery module.)
              testctl paramz
              testctl routez
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().Be(expected);
    }

    [Fact]
    public void Discovery_VersionCommand_WritesExpected()
    {
        // Arrange
        const string command = "--version";
        var plainWriter = new PlainWriter();
        var expected = """
            Module: testctl v1.0.0-testing123 (Test project)
            CLI-ified with <3 by: Comanche v1.1.1 (ne1410s © 2024)
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().Be(expected);
    }

    [Fact]
    public void Discovery_VersionCommand_WritesScarletHeart()
    {
        // Arrange
        const string command = "--version";
        const string expected = " <3 ";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expected, false, ConsoleColor.DarkRed, false));
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("--debug")]
    public void Discovery_VersionCommandWithBadFlag_WritesExpectedError(string incompatibleFlag)
    {
        // Arrange
        var command = "--version " + incompatibleFlag;
        var plainWriter = new PlainWriter();

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text().Should().Contain("--version: Command does not support --debug or --help");
    }

    [Fact]
    public void Discovery_VersionCommandNoAssemblyInfoVersion_UsesMainVersion()
    {
        // Arrange
        const string command = "--version";
        const string expected = "Module: testctl v1.0.0.0 (Test project)";
        var plainWriter = new PlainWriter();

        // Act
        E2E.Run(command, plainWriter, asm: new E2E.InfolessAssembly());

        // Assert
        plainWriter.Text(true).Should().Contain(expected);
    }

    [Theory]
    [InlineData("disco dox")]
    [InlineData("disco dox --help")]
    public void Discovery_DocumentedModuleEmptyOrHelpCommand_WritesExpected(string command)
    {
        // Arrange
        var plainWriter = new PlainWriter();
        var expected = """
            Module: testctl disco dox (Tests doc gen.)
            Methods: testctl disco dox greet (Send a greeting.)
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().Be(expected);
    }

    [Fact]
    public void Discovery_DocumentedMethodHelpCommand_WritesExpected()
    {
        // Arrange
        const string command = "disco dox greet --help";
        var plainWriter = new PlainWriter();
        var expected = """
            Module: testctl disco dox (Tests doc gen.)
            Method: testctl disco dox greet (Send a greeting.)
            Parameters:
              --name (-n) [string] (The greetee.)
              --dicto [IDictionary<string, int> = null] (The numbers dictionary.)
            Returns: [string] (A greeting.)
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().Be(expected);
    }
}
