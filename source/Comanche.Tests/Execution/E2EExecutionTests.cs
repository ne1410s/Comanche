// <copyright file="E2EExecutionTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Execution;

using System;
using Comanche.Tests.Console;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using E2E = TestHelper;

/// <summary>
/// Testing successful execution.
/// </summary>
public class E2EExecutionTests
{
    [Fact]
    public void Execution_NullServiceCollection_PlugsSingleton()
    {
        // Arrange
        var stubServices = new ServiceCollection();

        // Act
        _ = Discover.Go(services: stubServices);

        // Assert
        stubServices.ShouldContain(d => d.ServiceType == typeof(IConfiguration));
        stubServices.ShouldContain(d => d.ServiceType == typeof(IConsole));
        stubServices.ShouldContain(d => d.ServiceType == typeof(ComanchePalette));
    }

    [Fact]
    public void Execution_CustomConsole_DoesNotInjectDefault()
    {
        // Arrange
        var stubServices = new ServiceCollection();
        var console = E2E.DefaultPalette.GetMockConsole().Object;

        // Act
        _ = E2E.Run(console: console, services: stubServices);

        // Assert
        stubServices.ShouldNotContain(d => d.ImplementationType == typeof(ConsoleWriter));
    }

    [Fact]
    public void Execution_NoDeepValidTerms_WritesExpected()
    {
        // Arrange
        var stubServices = new ServiceCollection();
        var plainWriter = new PlainWriter();

        // Act
        _ = E2E.Run(console: plainWriter, services: stubServices);

        // Assert
        plainWriter.Text(false).ShouldContain("testctl disco (Discovery module.)");
    }

    [Fact]
    public void Execution_Throw_WritesExpectedVerbatim()
    {
        // Arrange
        const string command = "exec throw";
        var plainWriter = new PlainWriter();
        var expected = """

            Exception:
            [ArithmeticException] Overflow or underflow in the arithmetic operation.

            Note:
            Run again with --debug for more detail.


            """.Normalise(false);

        // Act
        _ = E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(false).ShouldBe(expected);
    }

    [Fact]
    public void Execution_ThrowDebug_WritesExpected()
    {
        // Arrange
        const string command = "exec throw --debug";
        var plainWriter = new PlainWriter();
        var expected = """
            Exception: [ArithmeticException] Overflow or underflow in the arithmetic operation.
            Stack Trace: at Comanche.Tests
            """.Normalise(true);

        // Act
        _ = E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).ShouldStartWith(expected);
    }

    [Fact]
    public void Execution_ThrowStacklessDebug_WritesExpected()
    {
        // Arrange
        const string command = "exec throw-stackless --debug";
        var plainWriter = new PlainWriter();
        var expected = """

            Exception:
            [StacklessException] Exception of type 'Comanche.Tests.TestHelper+StacklessException' was thrown.
            
            Stack Trace:
            
            
            
            """.Normalise(false);

        // Act
        _ = E2E.Run(command, plainWriter);

        // Assert
        var actualText = plainWriter.Text(false);
        actualText.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null, "dev")]
    [InlineData("Development", "dev")]
    [InlineData("NonExisting", null)]
    [InlineData("Custom", "custom")]
    public void Execution_ModuleInjectedConfig_ReturnsExpected(string? environmentName, string? expected)
    {
        // Arrange
        const string command = "exec get-var";
        Environment.SetEnvironmentVariable(Discover.EnvironmentKey, environmentName);

        // Act
        var actual = E2E.Run(command);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Execution_ComplexObject_WritesExpectedJson()
    {
        // Arrange
        const string command = "exec write-json --my-int 42";
        var plainWriter = new PlainWriter();
        var expected = """
            {
              "myInt": 42,
              "myString": "'42'"
            }
            """.Normalise(true);

        // Act
        _ = E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).ShouldBe(expected);
    }

    [Fact]
    public void Execution_ProblematicJson_UsesObjectToString()
    {
        // Arrange
        const string command = "exec json-err";
        var plainWriter = new PlainWriter();
        var expectedObject = new JsonError { MyBool = true, MyString = "hi" };
        var expectedOutput = $"{expectedObject}\n";

        // Act
        var actual = E2E.Run(command, plainWriter);

        // Assert
        actual.ShouldBeEquivalentTo(expectedObject);
        plainWriter.Text(false).ShouldBe(expectedOutput);
    }

    [Fact]
    public void Execution_AsyncWithValue_ReturnsExpected()
    {
        // Arrange
        const string command = "exec get-async";
        const int expected = 43;

        // Act
        var actual = E2E.Run(command);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Execution_AsyncTaskOnly_ReturnsNull()
    {
        // Arrange
        const string command = "exec do-async";

        // Act
        var actual = E2E.Run(command);

        // Assert
        actual.ShouldBeNull();
    }
}
