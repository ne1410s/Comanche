using DemoLibrary.Validation;

namespace DemoLibrary.Tests.Validation
{
    public class InvalidItemTests
    {
        [Fact]
        public void Ctor_WithProperties_RetainsValues()
        {
            // Arrange
            const string property = "MyProp";
            const string errorMessage = "No good!";
            const int value = 1;

            // Act
            var result = new InvalidItem(property, errorMessage, value);

            // Assert
            result.Property.Should().Be(property);
            result.ErrorMessage.Should().Be(errorMessage);
            result.AttemptedValue.Should().Be(value);
        }
    }
}
