using DemoLibrary.Api.Models;

namespace DemoLibrary.Tests.Api
{
    public class HttpErrorBodyTests
    {
        [Fact]
        public void Ctor_WithProperties_RetainsValues()
        {
            // Arrange
            const string type = "mytype";
            const string message = "mymessage";

            // Act
            var result = new HttpErrorBody(type, message);

            // Assert
            result.Type.Should().Be(type);
            result.Message.Should().Be(message);
        }
    }
}
