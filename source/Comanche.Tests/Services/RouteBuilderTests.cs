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
        private static readonly Assembly TestCli = Assembly.GetAssembly(typeof(CliTest.NumbersModule))!;

        [Fact]
        public void BuildRoutes_ThisAssembly_LimitsTypesExpected()
        {
            // Arrange
            var assembly = Assembly.GetEntryAssembly()!;
            var sut = new RouteBuilder();

            // Act
            var routes = sut.BuildRoutes(assembly);

            // Assert
            routes.Should().AllSatisfy(r => r.Key.StartsWith("unittestclient"));
        }


        [Fact]
        public void BuildRoutes_TestAssembly_DoesNotExposeNonStaticModule()
        {
            // Arrange
            var sut = new RouteBuilder();
            _ = NumbersModule.Do();
            _ = NumbersModule.Unexposed.Do();
            _ = NumbersModule.Unexposed2.Do();

            // Act
            var routes = sut.BuildRoutes(TestCli);

            // Assert
            routes.Keys.Should().Contain("num|alg|derive")
                .And.Contain("num|d")
                .And.NotContain("num|unexposed|do")
                .And.NotContain("num|unexposed2|do");
        }
    }
}