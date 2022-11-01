﻿// <copyright file="SessionTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Comanche.OldNBusted;
using Comanche.Services;

namespace Comanche.Tests
{
    public class SessionTests
    {
        private static readonly Assembly TestCli = Assembly.GetAssembly(typeof(CliTest.NumbersModule))!;
        private static readonly Assembly LocalCli = Assembly.GetExecutingAssembly();

        [Fact]
        public void Route_HelpRequested_ReturnsNull()
        {
            // Arrange & Act
            object? result = Route("/?");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Route_HelpRequested_WritesStandardEntry()
        {
            // Arrange
            ConsoleWriter writer = new();
            string[] expectedEntries = new[]
            {
                $"The following commands are available:{Environment.NewLine}  > num",
            };

            // Act
            Route("/?", writer: writer);

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
            ConsoleWriter writer = new();
            const string command = "num alg derive splar -h";
            string[] expectedErrors = new[]
            {
                $"Error invoking command '{command}'. Command not recognised. "
                    + $"Suggestions:{Environment.NewLine}  > num alg derive",
            };

            // Act
            Route(command, writer: writer);

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
            const string command = "num alg";

            // Act
            Route(command);

            // Assert
            Environment.ExitCode.Should().Be(1);
        }

        [Fact]
        public void Route_ExecInvalidChild_SetsErrorCode()
        {
            // Arrange
            const string command = "num alg derive splar";

            // Act
            Route(command);

            // Assert
            Environment.ExitCode.Should().Be(1);
        }

        [Fact]
        public void Route_ParameterError_SetsErrorCode()
        {
            // Arrange
            const string command = "num check -os false";

            // Act
            _ = Route(command);

            // Assert
            Environment.ExitCode.Should().Be(1);
        }

        [Fact]
        public void Route_DefaultArgs_TruncatesFirst()
        {
            // Arrange
            ConsoleWriter writer = new();
            string[] args = Environment.GetCommandLineArgs();

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
            string command = $"num check --myStr {input}";

            // Act
            object? actual = Route(command);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Route_ValidProcess_WritesStandardEntry()
        {
            // Arrange
            ConsoleWriter writer = new();
            string[] expectedEntries = new[] { "2" };

            // Act
            Route("num alg derive --firstNumber 1 --second 1", writer: writer);

            // Assert
            writer.Entries.Should().BeEquivalentTo(expectedEntries);
        }

        [Fact]
        public void Route_SubModuleWithParams_ReturnsExpected()
        {
            // Arrange
            const string command = "num alg derive --firstNumber 1 --second 1";

            // Act
            object? actual = Route(command);

            // Assert
            actual.Should().Be(2);
        }

        [Fact]
        public void Route_InvalidConfiguration_ReturnsNull()
        {
            // Arrange
            string[]? args = (string[]?)null;

            // Act
            object? actual = Session.Route(args);

            // Assert
            actual.Should().BeNull();
        }

        [Theory]
        [InlineData("test")]
        [InlineData(@"backs\ash")]
        [InlineData("forwards/ash")]
        public void Route_VaryingStringParameterChars_ReturnsExpected(string input)
        {
            // Arrange
            var args = $"testmethods empty -p1 {input} -p2";
            var expected = $"{input}{true}";

            // Act
            var actual = Route(args, LocalCli);

            // Assert
            actual.Should().Be(expected);
        }

        private static object? Route(string consoleInput, Assembly? assembly = null, IOutputWriter? writer = null)
        {
            Environment.ExitCode = 0;
            return Session.Route(Regex.Split(consoleInput, "\\s+"), assembly ?? TestCli, writer);
        }
    }
}