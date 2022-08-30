using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Comanche.Services;

namespace Comanche.Tests;

public class SessionTests
{
    private static readonly Assembly TestCli = Assembly.GetAssembly(typeof(CliTest.NumbersModule))!;

    [Fact]
    public void Route_HelpRequested_ReturnsNull()
    {
        // Arrange & Act
        var result = Route("/?");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Route_HelpRequested_WritesStandardEntry()
    {
        // Arrange
        var writer = new ConsoleWriter();
        var expectedEntries = new[] { $"The following commands are available:{Environment.NewLine}  > num" };

        // Act
        Route("/?", writer);

        // Assert
        writer.Entries.Should().BeEquivalentTo(expectedEntries);
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    [InlineData("/?")]
    public void Route_HelpRequestedAtRoot_DoesNotSetErrorCode(string helpCommand)
    {
        // Arrange & Act
        Route(helpCommand);

        // Assert
        Environment.ExitCode.Should().Be(0);
    }

    [Theory]
    [InlineData("num --help")]
    [InlineData("num -h")]
    [InlineData("num /?")]
    public void Route_HelpRequestedAtTop_DoesNotSetErrorCode(string helpCommand)
    {
        // Arrange & Act
        Route(helpCommand);

        // Assert
        Environment.ExitCode.Should().Be(0);
    }

    [Theory]
    [InlineData("num --help")]
    [InlineData("num -h")]
    [InlineData("num /?")]
    public void Route_HelpRequestedAtChild_DoesNotSetErrorCode(string helpCommand)
    {
        // Arrange & Act
        Route(helpCommand);

        // Assert
        Environment.ExitCode.Should().Be(0);
    }

    [Theory]
    [InlineData("num alg --help")]
    [InlineData("num alg -h")]
    [InlineData("num alg /?")]
    public void Route_HelpRequestedNoMatchButValidChild_DoesNotSetErrorCode(string helpCommand)
    {
        // Arrange & Act
        Route(helpCommand);

        // Assert
        Environment.ExitCode.Should().Be(0);
    }

    [Theory]
    [InlineData("num alg derive splar --help")]
    [InlineData("num alg derive splar -h")]
    [InlineData("num alg derive splar /?")]
    public void Route_HelpRequestedAtInvalidChild_SetsErrorCode(string helpCommand)
    {
        // Arrange & Act
        Route(helpCommand);

        // Assert
        Environment.ExitCode.Should().Be(1);
    }

    [Fact]
    public void Route_WithError_WritesErrorEntry()
    {
        // Arrange
        var writer = new ConsoleWriter();
        var command = "num alg derive splar -h";
        var expectedErrors = new[] { $"Error invoking command '{command}'. Command not recognised. "
            + $"Suggestions:{Environment.NewLine}  > num alg derive" };

        // Act
        Route(command, writer);

        // Assert
        writer.ErrorEntries.Should().BeEquivalentTo(expectedErrors);
    }

    [Theory]
    [InlineData("/?")]
    [InlineData("unittestclient|run|/?")]
    public void Route_HelpFromNoXmlDoc_NoText(string parameterPsv)
    {
        // Arrange, Act, Assert
        Session.Route(parameterPsv.Split('|')).Should().BeNull();
    }

    [Fact]
    public void Route_ExecNoMatchButValidChild_SetsErrorCode()
    {
        // Arrange
        var command = "num alg";

        // Act
        Route(command);

        // Assert
        Environment.ExitCode.Should().Be(1);
    }

    [Fact]
    public void Route_ExecInvalidChild_SetsErrorCode()
    {
        // Arrange
        var command = "num alg derive splar";

        // Act
        Route(command);

        // Assert
        Environment.ExitCode.Should().Be(1);
    }

    [Fact]
    public void Route_ParameterError_SetsErrorCode()
    {
        // Arrange
        var command = $"num check -os false";

        // Act
        _ = Route(command);

        // Assert
        Environment.ExitCode.Should().Be(1);
    }

    [Fact]
    public void Route_DefaultArgs_TruncatesFirst()
    {
        // Arrange
        var writer = new ConsoleWriter();
        var args = Environment.GetCommandLineArgs();

        // Act
        _ = Session.Route(outputWriter: writer);

        writer.ErrorEntries[0].Should()
            .NotStartWith($"Error invoking command '{args[0]}")
            .And.StartWith($"Error invoking command '{args[1]}");
    }

    [Theory]
    [InlineData("4.2", true)]
    [InlineData("0", true)]
    [InlineData("trace", false)]
    public void Route_WithParams_ReturnsExpected(string input, bool expected)
    {
        // Arrange
        var command = $"num check --myStr {input}";

        // Act
        var actual = Route(command);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Route_ValidProcess_WritesStandardEntry()
    {
        // Arrange
        var writer = new ConsoleWriter();
        var expectedEntries = new[] { "2" };

        // Act
        Route("num alg derive --firstNumber 1 --second 1", writer);

        // Assert
        writer.Entries.Should().BeEquivalentTo(expectedEntries);
    }

    [Fact]
    public void Route_SubModuleWithParams_ReturnsExpected()
    {
        // Arrange
        var command = "num alg derive --firstNumber 1 --second 1";

        // Act
        var actual = Route(command);

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void Route_InvalidConfiguration_ReturnsNull()
    {
        // Arrange
        var args = (string[]?)null;

        // Act
        var actual = Session.Route(args);

        // Assert
        actual.Should().BeNull();
    }

    private static object? Route(string consoleInput, IOutputWriter? writer = null)
    {
        Environment.ExitCode = 0;
        return Session.Route(Regex.Split(consoleInput, "\\s+"), TestCli, writer);
    }
}
