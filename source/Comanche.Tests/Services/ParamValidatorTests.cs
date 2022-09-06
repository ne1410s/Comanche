using System;
using Comanche.Exceptions;
using Comanche.Models;
using Comanche.Services;

namespace Comanche.Tests.Services
{
    public class ParamValidatorTests
    {
        [Fact]
        public void Validate_ParamsWithDefaultAndNotSupplied_RevertToDefault()
        {
            // Arrange
            var sut = new ParamValidator();
            var route = new MethodRoute(TestMethods.Empty_Info, new());

            // Act
            var parameters = sut.Validate(route);

            // Assert
            parameters.Should().BeEquivalentTo(new object[] { "default", false });
        }

        [Fact]
        public void Validate_ParamsWithDefaultAndSuppliedEmpty_RevertToExpected()
        {
            // Arrange
            var sut = new ParamValidator();
            var route = new MethodRoute(TestMethods.Empty_Info, new()
            {
                ["p1"] = "",
                ["p2"] = "",
            });

            // Act
            var parameters = sut.Validate(route);

            // Assert
            parameters.Should().BeEquivalentTo(new object[] { "", true });
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        public void Validate_StringParamsSuppliedWhitespace_ValueRetained(string whitespace)
        {
            // Arrange
            var sut = new ParamValidator();
            var route = new MethodRoute(TestMethods.Empty_Info, new()
            {
                ["p1"] = whitespace
            });

            // Act
            var parameters = sut.Validate(route);

            // Assert
            parameters.Should().BeEquivalentTo(new object[] { whitespace, false });
        }

        [Theory]
        [InlineData("n1")]
        [InlineData("num1")]
        public void Validate_InconvertibleParam_ThrowsException(string param1Ref)
        {
            // Arrange
            var sut = new ParamValidator();
            var route = new MethodRoute(TestMethods.Add_Info, new()
            {
                [param1Ref] = "se7en",
                ["num2"] = "33",
            });
            var expected = "--num1 (-n1) is invalid. Input string was not in a correct format.";

            // Act
            var act = () => sut.Validate(route);

            // Assert
            act.Should().ThrowExactly<ParamsException>()
                .WithMessage("Parameters not valid:*")
                .Which.Errors.Should().BeEquivalentTo(new string[] { expected });
        }

        [Theory]
        [InlineData("n1")]
        [InlineData("num1")]
        public void Validate_ConvertibleParams_ReturnsExpected(string param1Ref)
        {
            // Arrange
            var sut = new ParamValidator();
            var route = new MethodRoute(TestMethods.Add_Info, new()
            {
                [param1Ref] = "7",
                ["num2"] = "33",
                ["n3"] = "8"
            });

            // Act
            var parameters = sut.Validate(route);

            // Assert
            parameters.Should().BeEquivalentTo(new object[] { 7, 33, 8 });
        }

        [Fact]
        public void Validate_Paramless_ReturnsNull()
        {
            // Arrange
            var sut = new ParamValidator();
            var route = new MethodRoute(TestMethods.ParamlessMethod_Info, new());

            // Act
            var parameters = sut.Validate(route);

            // Assert
            parameters.Should().BeNull();
        }

        [Fact]
        public void Validate_UnknownParameter_ThrowsException()
        {
            // Arrange
            var sut = new ParamValidator();
            var route = new MethodRoute(TestMethods.Add_Info, new()
            {
                ["num1"] = "7",
                ["num2"] = "33",
                ["num444556"] = "444556",
            });
            var expected = "'num444556' is unrecognised.";

            // Act
            var act = () => sut.Validate(route);

            // Assert
            act.Should().ThrowExactly<ParamsException>()
                .WithMessage("Parameters not valid:*")
                .Which.Errors.Should().BeEquivalentTo(new string[] { expected });
        }

        [Fact]
        public void Validate_MissingRequiredParameter_ThrowsException()
        {
            // Arrange
            var sut = new ParamValidator();
            var route = new MethodRoute(TestMethods.Add_Info, new()
            {
                ["num2"] = "33",
            });
            var expected = "--num1 (-n1) is required";

            // Act
            var act = () => sut.Validate(route);

            // Assert
            act.Should().ThrowExactly<ParamsException>()
                .WithMessage("Parameters not valid:*")
                .Which.Errors.Should().BeEquivalentTo(new string[] { expected });
        }

        [Fact]
        public void Validate_DuplicateParameter_ThrowsException()
        {
            // Arrange
            var sut = new ParamValidator();
            var route = new MethodRoute(TestMethods.Add_Info, new()
            {
                ["n1"] = "7",
                ["num2"] = "33",
                ["num1"] = "444556",
            });
            var expected = "'num1' is already supplied.";

            // Act
            var act = () => sut.Validate(route);

            // Assert
            act.Should().ThrowExactly<ParamsException>()
                .WithMessage("Parameters not valid:*")
                .Which.Errors.Should().BeEquivalentTo(new string[] { expected });
        }

        [Fact]
        public void Validate_MultipleInvalidParameters_ThrowsException()
        {
            // Arrange
            var sut = new ParamValidator();
            var route = new MethodRoute(TestMethods.Add_Info, new()
            {
                ["n1"] = "se7en",
                ["num2"] = "3hree",
                ["num3"] = "4our",
            });
            var errors = new string[]
            {
            "--num1 (-n1) is invalid. Input string was not in a correct format.",
            "--num2 is invalid. Input string was not in a correct format.",
            "--num3 (-n3) is invalid. Input string was not in a correct format."
            };
            var expectedMessage = $"Parameters not valid:{Environment.NewLine}  > "
                + string.Join($"{Environment.NewLine}  > ", errors);

            // Act
            var act = () => sut.Validate(route);

            // Assert
            act.Should().ThrowExactly<ParamsException>().WithMessage(expectedMessage);
        }
    }
}