// <copyright file="HelpGeneratorTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using Comanche.Models;
using Comanche.Services;

namespace Comanche.Tests.Services
{
    /// <summary>
    /// Tests for the <see cref="HelpGenerator"/> class.
    /// </summary>
    public class HelpGeneratorTests
    {
        [Fact]
        public void GenerateHelp_TestCliMethodWithXmlSummary_ContainsSummaryText()
        {
            // Arrange
            MethodInfo methodInfo = typeof(CliTest.NumbersModule)
                .GetMethod(nameof(CliTest.NumbersModule.Check))!;
            MethodHelp methodHelp = new(methodInfo);
            HelpGenerator sut = new(Assembly.GetAssembly(typeof(CliTest.NumbersModule))!);

            // Act
            string helpText = sut.GenerateHelp(methodHelp);

            // Assert
            helpText.Should().Contain("Does a check.");
        }

        [Fact]
        public void GenerateHelp_TestAsmMethodWithMultilineSummary_ContainsSummaryText()
        {
            // Arrange
            MethodHelp methodHelp = new(TestMethods.Parse_Info);
            HelpGenerator sut = new(Assembly.GetAssembly(typeof(TestMethods))!);

            // Act
            string helpText = sut.GenerateHelp(methodHelp);

            // Assert
            helpText.Should().Contain("Does some parsing. Nothing more.");
        }

        [Fact]
        public void GenerateHelp_ModuleHelp_ReturnsSuggestedModules()
        {
            // Arrange
            ModuleHelp moduleHelp = new(new HashSet<string> { "one", "two" });
            HelpGenerator sut = new(Assembly.GetAssembly(typeof(CliTest.NumbersModule))!);

            // Act
            string helpText = sut.GenerateHelp(moduleHelp);

            // Assert
            helpText.Should().Be($"The following commands are available:{Environment.NewLine}  > "
                + $"one{Environment.NewLine}  > two");
        }

        [Fact]
        public void GenerateHelp_MethodHelp_ProducesExpectedText()
        {
            // Arrange
            MethodInfo methodInfo = typeof(CliTest.NumbersModule)
                .GetMethod(nameof(CliTest.NumbersModule.Check))!;
            MethodHelp methodHelp = new(methodInfo);
            HelpGenerator sut = new(Assembly.GetAssembly(typeof(CliTest.NumbersModule))!);
            const string expectedText = @"method: Check
summary: Does a check.
remarks: Need to know basis.
returns: True if a number.
parameters:
  name: myStr
  name: otherThing
";

            // Act
            string helpText = sut.GenerateHelp(methodHelp);

            // Assert
            helpText.Should().Contain(expectedText);
        }

        [Fact]
        public void GenerateHelp_ParameterlessMethodHelp_ProducesExpectedText()
        {
            // Arrange
            MethodInfo methodInfo = typeof(CliTest.NumbersModule).GetMethod(nameof(CliTest.NumbersModule.Do))!;
            MethodHelp methodHelp = new(methodInfo);
            HelpGenerator sut = new(Assembly.GetAssembly(typeof(CliTest.NumbersModule))!);
            const string expectedText = @"method: Do (d)
summary: Does.
returns: Unity.
";

            // Act
            string helpText = sut.GenerateHelp(methodHelp);

            // Assert
            helpText.Should().Contain(expectedText);
        }
    }
}