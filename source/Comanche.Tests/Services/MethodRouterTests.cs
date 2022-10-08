// <copyright file="MethodRouterTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Comanche.Exceptions;
using Comanche.Models;
using Comanche.Services;

namespace Comanche.Tests.Services
{
    public class MethodRouterTests
    {
        [Fact]
        public void GetClosestRoute_IncompleteRoute_ReturnsMostCompleteValid()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            string? result = sut.GetClosestRoute("test|pass|other", new[] { "test|pass|oth" });

            // Assert
            result.Should().Be("test|pass");
        }

        [Fact]
        public void GetClosestRoute_NoMatches_ReturnsNull()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            string? result = sut.GetClosestRoute("test", new[] { "pass" });

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetClosestRoute_NoRoutes_ReturnsNull()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            string? result = sut.GetClosestRoute("test", Array.Empty<string>());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetClosestRoute_OnlySiblings_ReturnsCommonRoute()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            string? result = sut.GetClosestRoute("t1|t3", new[] { "t1|t2a", "t1|t2b" });

            // Assert
            result.Should().Be("t1");
        }

        [Fact]
        public void GetClosestRoute_RouteIsValid_ReturnsValidRoute()
        {
            // Arrange
            MethodRouter sut = new();
            const string validRoute = "test|pass";

            // Act
            string? result = sut.GetClosestRoute(validRoute, new[] { "test", "pass", validRoute, });

            // Assert
            result.Should().Be(validRoute);
        }

        [Fact]
        public void GetClosestRoute_WithValidAncestor_ReturnsAncestorRoute()
        {
            // Arrange
            MethodRouter sut = new();
            const string validRoute = "test|pass";
            const string attemptedRoute = validRoute + "|other";

            // Act
            string? result = sut.GetClosestRoute(attemptedRoute, new[] { "test", "pass", validRoute, });

            // Assert
            result.Should().Be(validRoute);
        }

        [Fact]
        public void GetOptions_InvalidRoute_ReturnsTopLevelOptions()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            HashSet<string> opts = sut.GetOptions("top999", new[] { "top1|mid1|low1", "top2|low2", "top3" });

            // Assert
            opts.Should().BeEquivalentTo(new[] { "top1", "top2", "top3" });
        }

        [Fact]
        public void GetOptions_NullRoute_ReturnsTopLevelOptions()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            HashSet<string> opts = sut.GetOptions(null, new[] { "top1|mid1|low1", "top2|low2", "top3" });

            // Assert
            opts.Should().BeEquivalentTo(new[] { "top1", "top2", "top3" });
        }

        [Fact]
        public void GetOptions_OnlySiblings_ReturnsParentalSiblings()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            HashSet<string> opts = sut.GetOptions("top1|mid1|low99", new[] { "top1|mid1|low1", "top1|mid1|lower2" });

            // Assert
            opts.Should().BeEquivalentTo(new[] { "low1", "lower2" });
        }

        [Fact]
        public void GetOptions_ValidParent_ReturnsParentalSiblings()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            HashSet<string> opts = sut.GetOptions("top1|mid3", new[] { "top1|mid1|low1", "top1|mid2", "top1" });

            // Assert
            opts.Should().BeEquivalentTo(new[] { "mid1", "mid2" });
        }

        [Fact]
        public void GetOptions_ValidRoute_ReturnsSiblings()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            HashSet<string> opts = sut.GetOptions("top1", new[] { "top1|mid1|low1", "top1|mid2", "top1" });

            // Assert
            opts.Should().BeEquivalentTo(new[] { "mid1", "mid2" });
        }

        [Fact]
        public void GetOptions_ValidSingle_ReturnsSingle()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            HashSet<string> opts = sut.GetOptions("top1|mid2", new[] { "top1|mid1|low1", "top1|mid2", "top1" });

            // Assert
            opts.Should().BeEquivalentTo(new[] { "mid2" });
        }

        [Fact]
        public void LocateMethod_WithRootHelpKey_ReturnsMethodHelp()
        {
            // Arrange
            MethodRouter sut = new();
            const string methodKey = "parse";
            Dictionary<string, MethodInfo> method = new() { [methodKey] = TestMethods.Parse_Info };

            // Act
            RouteResult result = sut.LocateMethod(new string[] { "/?" }, method);

            // Assert
            result.Should().BeOfType<ModuleHelp>()
                .Which.Modules.Should().BeEquivalentTo(new string[] { methodKey });
        }

        [Theory]
        [InlineData("-h")]
        [InlineData("--help")]
        [InlineData("/?")]
        public void LocateMethod_WithHelpKey_ReturnsMethodHelp(string helpKey)
        {
            // Arrange
            MethodRouter sut = new();
            const string methodKey = "parse";
            Dictionary<string, MethodInfo> method = new() { [methodKey] = TestMethods.Parse_Info };

            // Act
            RouteResult result = sut.LocateMethod(new string[] { methodKey, helpKey }, method);

            // Assert
            result.Should().BeOfType<MethodHelp>()
                .Which.Method.Should().BeSameAs(TestMethods.Parse_Info);
        }

        [Fact]
        public void LocateMethod_NoneSet_NoClosestRouteInException()
        {
            // Arrange
            MethodRouter sut = new();

            // Act
            Func<RouteResult> act = () => sut.LocateMethod(new[] { "bad1", "bad2" }, new());

            // Assert
            act.Should().Throw<RouteException>().Which.ClosestRoute.Should().BeNull();
        }

        [Fact]
        public void LocateMethod_ValidSingleAncestor_LocatesMethod()
        {
            // Arrange
            MethodRouter sut = new();
            const string ancestor = "mod1|mod2";
            string[] requested = new[] { "mod1", "mod2", "bad1" };
            Dictionary<string, MethodInfo> method = new() { [ancestor] = TestMethods.Parse_Info };

            // Act
            Func<RouteResult> act = () => sut.LocateMethod(requested, method);

            // Assert
            act.Should().Throw<RouteException>()
                .Where(ex => ex.ClosestRouteOpts.First() == "mod1 mod2")
                .And.RequestedRoute.Should().Be(string.Join('|', requested));
        }

        [Fact]
        public void LocateMethod_ValidAncestorSiblings_SiblingsListedInException()
        {
            // Arrange
            MethodRouter sut = new();
            const string ancestorSubMethod1 = "mod1|mod2|mod3a";
            const string ancestorSubMethod2 = "mod1|mod2|mod3b";
            string[] requested = new[] { "mod1", "mod2", "bad1" };
            Dictionary<string, MethodInfo> method = new()
            {
                [ancestorSubMethod1] = TestMethods.Parse_Info,
                [ancestorSubMethod2] = TestMethods.Parse_Info,
            };
            string expectedMessage = $"Command not recognised. Suggestions:{Environment.NewLine}  > " +
                $"mod1 mod2 mod3a{Environment.NewLine}  > mod1 mod2 mod3b";

            // Act
            Func<RouteResult> act = () => sut.LocateMethod(requested, method);

            // Assert
            act.Should().Throw<RouteException>()
                .WithMessage(expectedMessage)
                .Which.ClosestRouteOpts.Should().BeEquivalentTo(new[] { "mod1 mod2 mod3a", "mod1 mod2 mod3b" });
        }

        [Theory]
        [InlineData("mod1")]
        [InlineData("mod1|mod2|mod3")]
        public void LocateMethod_WithMethod_MethodFound(string modulePsv)
        {
            // Arrange
            MethodRouter sut = new();
            Dictionary<string, MethodInfo> method = new() { [modulePsv] = TestMethods.Parse_Info };

            // Act
            RouteResult result = sut.LocateMethod(modulePsv.Split('|'), method);

            // Assert
            result.Should().BeOfType<MethodRoute>()
                .Which.MethodInfo.Should().BeSameAs(method[modulePsv]);
        }

        [Fact]
        public void LocateMethod_WithParameters_ParametersFound()
        {
            // Arrange
            MethodRouter sut = new();
            const string module = "mod1";
            Dictionary<string, MethodInfo> method = new() { [module] = TestMethods.Parse_Info };
            Dictionary<string, string> expected = new() { ["force"] = string.Empty, ["c"] = "42" };

            // Act
            RouteResult result = sut.LocateMethod(new[] { module, "--force", "-c 42" }, method);

            // Assert
            result.Should().BeOfType<MethodRoute>()
                .Which.ActualParams.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void LocateMethod_WithParametersDuplicated_ThrowsException()
        {
            // Arrange
            MethodRouter sut = new();
            const string module = "mod1";
            Dictionary<string, MethodInfo> method = new() { [module] = TestMethods.Parse_Info };

            // Act
            Func<RouteResult> act = () => sut.LocateMethod(new[] { module, "-c 12", "-c 42" }, method);

            // Assert
            act.Should().Throw<ParamsException>().WithMessage("* appears more than once.");
        }

        [Fact]
        public void PlacateTestAssemblyCoverage()
        {
            _ = TestMethods.Parse(string.Empty);
            _ = TestMethods.Parse($"{3}");
            _ = TestMethods.Add(1, 2, 4);
            _ = TestMethods.ParamlessMethod();
        }
    }
}