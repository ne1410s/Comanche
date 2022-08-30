using DemoLibrary.Errors;
using DemoLibrary.Validation;

namespace DemoLibrary.Tests.Validation
{
    public class FluentValidatorTests
    {
        [Fact]
        public void AssertValid_NotValid_ThrowsExpectedError()
        {
            // Arrange
            var model = new TestModel { Name = "AJ" };
            var validator = new TestModelValidator();
            var expectedItems = new[]
            {
                new InvalidItem(nameof(model.Name), "Gimme a name", model.Name),
                new InvalidItem(nameof(model.Magnitude), "Gimme some magnitude", model.Magnitude),
            };

            // Act
            var act = () => validator.AssertValid(model);

            // Assert
            act.Should().ThrowExactly<ValidationError>()
                .WithMessage("Invalid instance received.")
                .Which.InvalidItems
                .Should().BeEquivalentTo(expectedItems);
        }

        [Fact]
        public void AssertValid_IsValid_DoesNotThrow()
        {
            // Arrange
            var model = new TestModel { Name = "AJP", Magnitude = 50 };
            var validator = new TestModelValidator();

            // Act
            var act = () => validator.AssertValid(model);

            // Assert
            act.Should().NotThrow();
        }
    }
}
