// <copyright file="E2EExecutionTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Features.Execution;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using E2E = TestHelper;

/// <summary>
/// Testing successful execution.
/// </summary>
public class E2EExecutionTests
{
    [Fact]
    public void Execution_EmptyServiceCollection_InjectsDefaults()
    {
        // Arrange
        var stubServices = new ServiceCollection();

        // Act
        E2E.Run(services: stubServices);

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
}
