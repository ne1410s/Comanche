using System;
using System.Collections.Generic;
using System.Reflection;
using Comanche.Models;
using Comanche.Services;

namespace Comanche.Tests.Services;

/// <summary>
/// Tests for the <see cref="HelpGenerator"/> class.
/// </summary>
public class HelpGeneratorTests
{
    [Fact]
    public void GenerateHelp_TestCliMethodWithXmlSummary_ContainsSummaryText()
    {
        // Arrange
        var methodInfo = typeof(CliTest.NumbersModule).GetMethod(nameof(CliTest.NumbersModule.Check))!;
        var methodHelp = new MethodHelp(methodInfo);
        var sut = new HelpGenerator(Assembly.GetAssembly(typeof(CliTest.NumbersModule))!);

        // Act
        var helpText = sut.GenerateHelp(methodHelp);

        // Assert
        helpText.Should().Contain("Does a check.");
    }

    [Fact]
    public void GenerateHelp_TestAsmMethodWithMultilineSummary_ContainsSummaryText()
    {
        // Arrange
        var methodHelp = new MethodHelp(TestMethods.Parse_Info);
        var sut = new HelpGenerator(Assembly.GetAssembly(typeof(TestMethods))!);

        // Act
        var helpText = sut.GenerateHelp(methodHelp);

        // Assert
        helpText.Should().Contain("Does some parsing. Nothing more.");
    }

    [Fact]
    public void GenerateHelp_ModuleHelp_ReturnsSuggestedModules()
    {
        // Arrange
        var moduleHelp = new ModuleHelp(new HashSet<string> { "one", "two" });
        var sut = new HelpGenerator(Assembly.GetAssembly(typeof(CliTest.NumbersModule))!);

        // Act
        var helpText = sut.GenerateHelp(moduleHelp);

        // Assert
        helpText.Should().Be($"The following commands are available:{Environment.NewLine}  > "
            + $"one{Environment.NewLine}  > two");
    }

    [Fact]
    public void GenerateHelp_MethodHelp_ProducesExpectedText()
    {
        // Arrange
        var methodInfo = typeof(CliTest.NumbersModule).GetMethod(nameof(CliTest.NumbersModule.Check))!;
        var methodHelp = new MethodHelp(methodInfo);
        var sut = new HelpGenerator(Assembly.GetAssembly(typeof(CliTest.NumbersModule))!);
        var expectedText = @"method: Check
summary: Does a check.
remarks: Need to know basis.
returns: True if a number.
parameters:
  name: myStr
  name: otherThing
";

        // Act
        var helpText = sut.GenerateHelp(methodHelp);

        // Assert
        helpText.Should().Contain(expectedText);
    }

    [Fact]
    public void GenerateHelp_ParameterlessMethodHelp_ProducesExpectedText()
    {
        // Arrange
        var methodInfo = typeof(CliTest.NumbersModule).GetMethod(nameof(CliTest.NumbersModule.Do))!;
        var methodHelp = new MethodHelp(methodInfo);
        var sut = new HelpGenerator(Assembly.GetAssembly(typeof(CliTest.NumbersModule))!);
        var expectedText = @"method: Do (d)
summary: Does.
returns: Unity.
";

        // Act
        var helpText = sut.GenerateHelp(methodHelp);

        // Assert
        helpText.Should().Contain(expectedText);
    }
}
