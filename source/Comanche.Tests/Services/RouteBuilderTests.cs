// <copyright file="RouteBuilderTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

using System.Reflection;
using Comanche.CliTest;
using Comanche.Services;

namespace Comanche.Tests.Services
{
    /// <summary>
    /// Tests for the <see cref="RouteBuilder"/> class.
    /// </summary>
    public class RouteBuilderTests
    {
        private static readonly Assembly TestCli
            = Assembly.GetAssembly(typeof(NumbersModule))!;

        [Fact]
        public void BuildRoutes_ThisAssembly_LimitsTypesExpected()
        {
            // Arrange
            Assembly assembly = Assembly.GetEntryAssembly()!;
            RouteBuilder sut = new();

            // Act
            System.Collections.Generic.Dictionary<string, MethodInfo> routes = sut.BuildRoutes(assembly);

            // Assert
            routes.Should().AllSatisfy(r => r.Key.StartsWith("unittestclient"));
        }

        [Fact]
        public void BuildRoutes_TestAssembly_DoesNotExposeNonStaticModule()
        {
            // Arrange
            RouteBuilder sut = new();
            _ = NumbersModule.Do();
            _ = NumbersModule.Unexposed.Do();
            _ = NumbersModule.Unexposed2.Do();

            // Act
            System.Collections.Generic.Dictionary<string, MethodInfo> routes = sut.BuildRoutes(TestCli);

            // Assert
            routes.Keys.Should().Contain("num|alg|derive")
                .And.Contain("num|d")
                .And.NotContain("num|unexposed|do")
                .And.NotContain("num|unexposed2|do");
        }
    }
}