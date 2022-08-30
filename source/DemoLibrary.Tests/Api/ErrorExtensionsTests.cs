using System.Runtime.Serialization;
using DemoLibrary.Api;
using DemoLibrary.Api.Models;
using DemoLibrary.Errors;
using DemoLibrary.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DemoLibrary.Tests.Api
{
    public class ErrorExtensionsTests
    {
        [Theory]
        [InlineData(typeof(StaticValidationError), 400)]
        [InlineData(typeof(AuthorisationError), 403)]
        [InlineData(typeof(ResourceMissingError), 404)]
        [InlineData(typeof(DataStateError), 422)]
        [InlineData(typeof(ValidationError), 422)]
        [InlineData(typeof(ServiceOrchestrationError), 422)]
        [InlineData(typeof(Exception), 500)]
        public void ToErrorCode_VaryingException_ReturnsExpectedCode(Type errorType, int expected)
        {
            // Arrange
            var ex = (Exception)FormatterServices.GetUninitializedObject(errorType);

            // Act
            var actual = ex.ToErrorCode();

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void ToItems_NoErrors_ReturnsEmpty()
        {
            // Arrange
            var input = new ModelStateDictionary();

            // Act
            var result = input.ToItems();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void ToItems_WithErrors_ReturnsExpectedItems()
        {
            // Arrange
            var input = new ModelStateDictionary();
            input.AddModelError("MyProp", "not valid");
            input.AddModelError("MyProp", "only on Sundays");
            input.AddModelError("MyText", "too long");
            var expected = new[]
            {
                new InvalidItem("MyProp", "not valid, only on Sundays", null),
                new InvalidItem("MyText", "too long", null),
            };

            // Act
            var result = input.ToItems();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToOutcome_GenericException_ConcealsOriginalMessage()
        {
            // Arrange
            var ex = new Exception("secret corporate stuff");
            var expectedBody = new HttpErrorBody(ex.GetType().Name, "An unexpected error occurred");
            var expected = new HttpErrorOutcome(500, expectedBody);

            // Act
            var actual = ex.ToOutcome();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToOutcome_SupportedError_RelaysOriginalMessage()
        {
            // Arrange
            var ex = new DataStateError("innocuous stuff");
            var expectedBody = new HttpErrorBody(ex.GetType().Name, ex.Message);
            var expected = new HttpErrorOutcome(422, expectedBody);

            // Act
            var actual = ex.ToOutcome();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToOutcome_ValidationError_ContainsModelData()
        {
            // Arrange
            var errors = new[]
            {
                new InvalidItem("MyProp", "no good", 3),
            };
            var ex = new StaticValidationError(errors);
            var expectedBody = new HttpErrorBody(ex.GetType().Name, ex.Message, errors);
            var expected = new HttpErrorOutcome(400, expectedBody);

            // Act
            var actual = ex.ToOutcome();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
