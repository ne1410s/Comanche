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
        Discover.Go(services: stubServices);

        // Assert
        stubServices.Should().Contain(d => d.ServiceType == typeof(IConfiguration));
        stubServices.Should().Contain(d => d.ServiceType == typeof(IConsole));
        stubServices.Should().Contain(d => d.ServiceType == typeof(ComanchePalette));
    }

    [Fact]
    public void Execution_CustomConsole_DoesNotInjectDefault()
    {
        // Arrange
        var stubServices = new ServiceCollection();
        var console = E2E.DefaultPalette.GetMockConsole().Object;

        // Act
        E2E.Run(console: console, services: stubServices);

        // Assert
        stubServices.Should().NotContain(d => d.ImplementationType == typeof(ConsoleWriter));
    }

    [Fact]
    public void Execution_Throw_WritesExpected()
    {
        // Arrange
        const string command = "exec throw";
        var plainWriter = new PlainWriter();
        var expected = """
            Exception: [ArithmeticException] Overflow or underflow in the arithmetic operation.
            Note: Run again with --debug for more detail.
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().Be(expected);
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
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().StartWith(expected);
    }

    [Fact]
    public void Execution_ThrowStacklessDebug_WritesExpected()
    {
        // Arrange
        const string command = "exec throw-stackless --debug";
        var plainWriter = new PlainWriter();
        var expected = """
            Exception: [StacklessException]
            Exception of type 'Comanche.Tests.TestHelper+StacklessException' was thrown.
            Stack Trace:
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().Be(expected);
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
        actual.Should().Be(expected);
    }

    [Fact]
    public void Execution_ComplexObject_WritesExpectedJson()
    {
        // Arrange
        const string command = "exec write-json --my-int 42";
        var plainWriter = new PlainWriter();
        var expected = """
            {{
              "myInt": 42,
              "myString": "'42'"
            }}
            """.Normalise(true);

        // Act
        E2E.Run(command, plainWriter);

        // Assert
        plainWriter.Text(true).Should().Be(expected);
    }
}
