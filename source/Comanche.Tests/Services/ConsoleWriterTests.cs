using System;
using System.IO;
using Comanche.Services;

namespace Comanche.Tests.Services
{
    /// <summary>
    /// Tests for the <see cref="ConsoleWriter"/> class.
    /// </summary>
    public class ConsoleWriterTests
    {
        [Fact]
        public void WriteLine_IsError_WritesToErrorStream()
        {
            // Arrange
            var writer = new StringWriter();
            var sut = new ConsoleWriter();
            Console.SetError(writer);

            // Act
            sut.WriteLine("foo", true);

            // Assert
            writer.ToString().Should().Be("foo" + Environment.NewLine);
        }

        [Fact]
        public void WriteLine_NotError_WritesToStandardStream()
        {
            // Arrange
            var writer = new StringWriter();
            var sut = new ConsoleWriter();
            Console.SetOut(writer);

            // Act
            sut.WriteLine("bar", false);

            // Assert
            writer.ToString().Should().Be("bar" + Environment.NewLine);
        }
    }
}