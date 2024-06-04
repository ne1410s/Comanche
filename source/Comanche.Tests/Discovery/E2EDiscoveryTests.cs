// <copyright file="E2EDiscoveryTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Discovery;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comanche.Tests.Console;
using Microsoft.Extensions.DependencyInjection;
using E2E = TestHelper;

public class E2EDiscoveryTests
{
    [Fact]
    public void Go_NoServices_DoesNotThrow()
    {
        // Arrange
        var nullServices = (IServiceCollection?)null;

        // Act
        var act = () => Discover.Go(nullServices);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Discovery_NoArgs_WritesExpectedError()
    {
        // Arrange
        var mockConsole = E2E.DefaultPalette.GetMockConsole();
        const string expected = "Invalid route: --port";

        // Act
        E2E.Run(console: mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expected, true, E2E.DefaultPalette.Error, true));
    }

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
              testctl exec
              testctl paramz
              testctl routez
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().Be(expected);
    }

    [Fact]
    public void Discovery_UnknownModule_WritesExpectedError()
    {
        // Arrange
        const string command = "just-not-a-module";
        const string expected = "Invalid route.";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expected, true, E2E.DefaultPalette.Error, true));
    }

    [Fact]
    public void Discovery_UnknownMethod_WritesExpectedError()
    {
        // Arrange
        const string command = "disco just-not-a-method";
        const string expected = "No such method.";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expected, true, E2E.DefaultPalette.Error, true));
    }

    [Fact]
    public void Discovery_UnknownParameter_WritesExpectedError()
    {
        // Arrange
        const string command = "disco dox greet --just-not-a-parameter";
        const string expected = "--just-not-a-parameter: unrecognised";
        var mockConsole = E2E.DefaultPalette.GetMockConsole();

        // Act
        E2E.Run(command, mockConsole.Object);

        // Assert
        mockConsole.Verify(m => m.Write(expected, true, E2E.DefaultPalette.Error, true));
    }

    [Fact]
    public void Discovery_VersionCommand_WritesExpectedVerbatim()
    {
        // Arrange
        const string command = "--version";
        var plainWriter = new PlainWriter();
        var expected = """

            Module:
            testctl v1.0.0-testing123 (Test project)
            
            CLI-ified with <3 by:
            Comanche v1.1.1 (ne1410s © 2024)
            

            """.Normalise(false);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        var actualText = plainWriter.Text(false);
        actualText.Should().Be(expected);
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
    [InlineData("/?")]
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
            Sub Modules: testctl disco dox e2eno-alias
            Methods:
              testctl disco dox greet (Send a greeting.)
              testctl disco dox uber-defaults
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
              --dicto [IDictionary<string, int32> = null] (The numbers dictionary.)
            Returns: [string] (A greeting.)
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "Hi, Bob #2!")]
    [InlineData(false, "Hi, Bob #!")]
    public void Discovery_InvokingMethod_ReturnsExpected(bool sendVals, string expected)
    {
        // Arrange
        var dicto = new Dictionary<string, int> { ["d"] = 1, ["e"] = -1 };

        // Act
        var actual = E2EDocumentedModule.GetGreeting("Bob", sendVals ? dicto : null);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task Discovery_LotsOfDefaultParams_WritesExpected()
    {
        // Arrange
        const string command = "disco dox greet uber-defaults --help";
        var plainWriter = new PlainWriter();
        var expected = """
            Module: testctl disco dox greet (Tests doc gen.)
            Method: testctl disco dox greet uber-defaults
            Parameters:
            --my-int [int32 = 3]
            --my-str [string = "hiya"]
            --my-day [DayOfWeek = Friday]
            --my-struct [TestStruct = null]
            --my-arr [int32?[] = null]
            Returns: [string]
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);
        await E2EDocumentedModule.UberDefaults();

        // Assert
        plainWriter.Text(true).Should().Be(expected);
    }

    [Fact]
    public void Discovery_ParamlessAndVoidReturnHelpCommand_WritesExpectedVerbatim()
    {
        // Arrange
        const string command = "disco dox e2eno-alias nested do --help";
        var plainWriter = new PlainWriter();
        var expected = """
            
            Module:
            testctl disco dox e2eno-alias nested (Nested module.)

            Method:
            testctl disco dox e2eno-alias nested do (JDI.)

            Returns:
            [<void>]

            
            """.Normalise(false);

        // Act
        E2E.Run(command, plainWriter);
        E2ENoAliasModule.E2ENestedModule.Do();

        // Assert
        var actualText = plainWriter.Text(false);
        actualText.Should().Be(expected);
    }

    [Fact]
    public void Discovery_PrimitiveParameters_LabelledAsExpected()
    {
        // Arrange
        const string command = "disco dox e2eno-alias invert --help";
        var plainWriter = new PlainWriter();
        var expected = """
            Parameters:
            --b [boolean]
            --l [int64]
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);
        E2ENoAliasModule.Invert(true, 0);

        // Assert
        plainWriter.Text(true).Should().Contain(expected);
    }

    [Fact]
    public void Discovery_NullableType_LabelledAsExpected()
    {
        // Arrange
        const string command = "disco dox e2eno-alias nullable --help";
        var plainWriter = new PlainWriter();
        var expected = """
            Parameters:
            --id [int64? = 4300]
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);
        E2ENoAliasModule.Nullable();

        // Assert
        plainWriter.Text(true).Should().Contain(expected);
    }
}
