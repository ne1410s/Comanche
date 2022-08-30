using DemoLibrary.Errors;
using DemoLibrary.Extensions;
using DemoLibrary.Tests.Validation;
using DemoLibrary.Validation;

namespace DemoLibrary.Tests.Extensions
{
    /// <summary>
    /// Tests for the <see cref="AssertionExtensions"/> class.
    /// </summary>
    public class AssertionExtensionsTests
    {
        [Fact]
        public void MustBePopulated_IsPopulated_DoesNotThrow()
        {
            // Arrange
            const string testObj = "hello";

            // Act
            var act = () => testObj.MustBePopulated();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustBePopulated_NotPopulated_ThrowsDataStateError()
        {
            // Arrange
            const string testObj = null!;

            // Act
            var act = () => testObj.MustBePopulated();

            // Assert
            act.Should().ThrowExactly<DataStateError>();
        }

        [Fact]
        public void MustBePopulated_NotPopulatedAndHasMessage_ExceptionContainsMessage()
        {
            // Arrange
            const string testObj = null!;
            const string message = "no greetings allowed!";

            // Act
            var act = () => testObj.MustBePopulated(message);

            // Assert
            act.Should().Throw<Exception>().WithMessage(message);
        }

        [Fact]
        public void MustBePopulated_NotPopulatedButExempt_DoesNotThrow()
        {
            // Arrange
            const string testObj = null!;

            // Act
            var act = () => testObj.MustBePopulated(unless: () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustBePopulated_PopulatedAndExempt_DoesNotThrow()
        {
            // Arrange
            const string testObj = null!;

            // Act
            var act = () => testObj.MustBePopulated(unless: () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustBePopulated_DefaultValueType_ThrowsDataStateError()
        {
            // Arrange
            const int testObj = default;

            // Act
            var act = () => testObj.MustBePopulated();

            // Assert
            act.Should().ThrowExactly<DataStateError>();
        }

        [Fact]
        public void MustBePopulated_NullableValueTypeWithValueDefault_DoesNotThrow()
        {
            // Arrange
            int? testObj = default(int);

            // Act
            var act = () => testObj.MustBePopulated();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustBeUnpopulated_IsPopulated_ThrowsDataStateError()
        {
            // Arrange
            const string testObj = "hello";

            // Act
            var act = () => testObj.MustBeUnpopulated();

            // Assert
            act.Should().ThrowExactly<DataStateError>();
        }

        [Fact]
        public void MustBeUnpopulated_IsPopulatedAndHasMessage_ExceptionContainsMessage()
        {
            // Arrange
            const string testObj = "hello";
            const string message = "no greetings allowed!";

            // Act
            var act = () => testObj.MustBeUnpopulated(message);

            // Assert
            act.Should().Throw<Exception>().WithMessage(message);
        }

        [Fact]
        public void MustBeUnpopulated_IsPopulatedButExempt_DoesNotThrow()
        {
            // Arrange
            const string testObj = "hello";

            // Act
            var act = () => testObj.MustBeUnpopulated(unless: () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustBeUnpopulated_NotPopulated_DoesNotThrow()
        {
            // Arrange
            const string testObj = null!;

            // Act
            var act = () => testObj.MustBeUnpopulated();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustBeUnpopulated_NotPopulatedAndExempt_DoesNotThrow()
        {
            // Arrange
            const string testObj = null!;

            // Act
            var act = () => testObj.MustBeUnpopulated(unless: () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustSerializeAs_MatchingObjects_DoesNotThrow()
        {
            // Arrange
            var object1 = new { Test = "123" };
            var object2 = new { Test = "123" };

            // Act
            var act = () => object1.MustSerializeAs(object2);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustSerializeAs_NotMatchingObjects_ThrowsDataStateError()
        {
            // Arrange
            var object1 = new { Test = "123" };
            var object2 = new { Test = "124" };

            // Act
            var act = () => object1.MustSerializeAs(object2);

            // Assert
            act.Should().Throw<DataStateError>();
        }

        [Fact]
        public void MustSerializeAs_NotMatchingObjectsWithMessage_ExceptionContainsMessage()
        {
            // Arrange
            var object1 = new { Test = "123" };
            var object2 = new { Test = "124" };
            const string message = "No match, bro";

            // Act
            var act = () => object1.MustSerializeAs(object2, message);

            // Assert
            act.Should().Throw<DataStateError>().WithMessage(message);
        }

        [Fact]
        public void MustSerializeAs_NotMatchingObjectsButExempt_DoesNotThrow()
        {
            // Arrange
            var object1 = new { Test = "123" };
            var object2 = new { Test = "124" };

            // Act
            var act = () => object1.MustSerializeAs(object2, unless: () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustNotSerializeAs_NotMatchingObjects_DoesNotThrow()
        {
            // Arrange
            var object1 = new { Test = "123" };
            var object2 = new { Test = "124" };

            // Act
            var act = () => object1.MustNotSerializeAs(object2);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustNotSerializeAs_MatchingObjects_ThrowsDataStateError()
        {
            // Arrange
            var object1 = new { Test = "123" };
            var object2 = new { Test = "123" };

            // Act
            var act = () => object1.MustNotSerializeAs(object2);

            // Assert
            act.Should().Throw<DataStateError>();
        }

        [Fact]
        public void MustNotSerializeAs_MatchingObjectsWithMessage_ExceptionContainsMessage()
        {
            // Arrange
            var object1 = new { Test = "123" };
            var object2 = new { Test = "123" };
            const string message = "No match, bro";

            // Act
            var act = () => object1.MustNotSerializeAs(object2, message);

            // Assert
            act.Should().Throw<DataStateError>().WithMessage(message);
        }

        [Fact]
        public void MustNotSerializeAs_MatchingObjectsButExempt_DoesNotThrow()
        {
            // Arrange
            var object1 = new { Test = "123" };
            var object2 = new { Test = "123" };

            // Act
            var act = () => object1.MustNotSerializeAs(object2, unless: () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustBe_AreEqual_DoesNotThrow()
        {
            // Arrange
            const int value = 2;

            // Act
            var act = () => value.MustBe(1 + 1);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustBe_NotEqual_ThrowsDataStateError()
        {
            // Arrange
            const int value = 5;

            // Act
            var act = () => value.MustBe(2 + 2);

            // Assert
            act.Should().ThrowExactly<DataStateError>();
        }

        [Fact]
        public void MustBe_NotEqualWithMessage_ExceptionContainsMessage()
        {
            // Arrange
            const int value = 5;
            const string message = "no dice";

            // Act
            var act = () => value.MustBe(2 + 2, message);

            // Assert
            act.Should().ThrowExactly<DataStateError>().WithMessage(message);
        }

        [Fact]
        public void MustBe_NotEqualButExempt_DoesNotThrow()
        {
            // Arrange
            const int value = 5;

            // Act
            var act = () => value.MustBe(2 + 2, unless: () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustNotBe_NotEqual_DoesNotThrow()
        {
            // Arrange
            var date1 = new DateTime(1985, 10, 26);
            var date2 = DateTime.Parse("1985/10/26").Date.AddHours(1);

            // Act
            var act = () => date1.MustNotBe(date2);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustNotBe_AreEqual_ThrowsDataStateError()
        {
            // Arrange
            var date1 = new DateTime(1985, 10, 26);
            var date2 = DateTime.Parse("1985/10/26").Date.AddHours(0);

            // Act
            var act = () => date1.MustNotBe(date2);

            // Assert
            act.Should().ThrowExactly<DataStateError>();
        }

        [Fact]
        public void MustNotBe_AreEqualWithMessage_ExceptionContainsMessage()
        {
            // Arrange
            var date1 = new DateTime(1985, 10, 26);
            var date2 = new DateTime(1985, 10, 26);
            const string message = "same, bro";

            // Act
            var act = () => date1.MustNotBe(date2, message);

            // Assert
            act.Should().ThrowExactly<DataStateError>().WithMessage(message);
        }

        [Fact]
        public void MustNotBe_AreEqualButExempt_DoesNotThrow()
        {
            // Arrange
            var date1 = new DateTime(1985, 10, 26);
            var date2 = new DateTime(1985, 10, 26);

            // Act
            var act = () => date1.MustNotBe(date2, unless: () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustAdhereTo_IsValid_DoesNotThrow()
        {
            // Arrange
            var model = new TestModel();
            var mockValidator = new Mock<IItemValidator<TestModel>>();

            // Act
            var act = () => model.MustAdhereTo(mockValidator.Object);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustAdhereTo_NotValid_ThrowsExpectedError()
        {
            // Arrange
            var model = new TestModel();
            var mockValidator = new Mock<IItemValidator<TestModel>>();
            mockValidator
                .Setup(m => m.AssertValid(It.IsAny<TestModel>()))
                .Throws(new ValidationError());

            // Act
            var act = () => model.MustAdhereTo(mockValidator.Object);

            // Assert
            act.Should().ThrowExactly<ValidationError>()
                .WithMessage("Invalid instance received.");
        }

        [Fact]
        public void MustAdhereTo_NotValidButExempt_DoesNotThrow()
        {
            // Arrange
            var model = new TestModel();
            var mockValidator = new Mock<IItemValidator<TestModel>>();
            mockValidator
                .Setup(m => m.AssertValid(It.IsAny<TestModel>()))
                .Throws(new ValidationError());

            // Act
            var act = () => model.MustAdhereTo(mockValidator.Object, () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustExist_IsDefault_ThrowsResourceMissingError()
        {
            // Arrange
            TestModel testObj = null!;

            // Act
            var act = () => testObj.MustExist();

            // Assert
            act.Should().ThrowExactly<ResourceMissingError>();
        }

        [Fact]
        public void MustExist_NotDefault_DoesNotThrow()
        {
            // Arrange
            TestModel testObj = new();

            // Act
            var act = () => testObj.MustExist();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustExist_IsDefaultNoMessage_ExceptionContainsDefaultMessage()
        {
            // Arrange
            bool? testObj = null;

            // Act
            var act = () => testObj.MustExist();

            // Assert
            act.Should().Throw<Exception>().WithMessage("Resource not found");
        }

        [Fact]
        public void MustExist_IsDefaultWithMessage_ExceptionContainsMessage()
        {
            // Arrange
            bool? testObj = null;
            const string message = "just no";

            // Act
            var act = () => testObj.MustExist(message);

            // Assert
            act.Should().Throw<Exception>().WithMessage(message);
        }

        [Fact]
        public void MustExist_IsDefaultButExempt_DoesNotThrow()
        {
            // Arrange
            bool? testObj = null;

            // Act
            var act = () => testObj.MustExist(unless: () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustBeAllowed_NotAllowed_ThrowsAuthorisationError()
        {
            // Arrange
            const bool isAllowed = false;

            // Act
            var act = () => isAllowed.MustBeAllowed();

            // Assert
            act.Should().ThrowExactly<AuthorisationError>();
        }


        [Fact]
        public void MustBeAllowed_NotAllowedNoMessage_ExceptionContainsDefaultMessage()
        {
            // Arrange
            const bool isAllowed = false;

            // Act
            var act = () => isAllowed.MustBeAllowed();

            // Assert
            act.Should().Throw<Exception>().WithMessage("Forbidden");
        }

        [Fact]
        public void MustBeAllowed_NotAllowedWithMessage_ExceptionContainsMessage()
        {
            // Arrange
            const bool isAllowed = false;
            const string message = "errr..";

            // Act
            var act = () => isAllowed.MustBeAllowed(message);

            // Assert
            act.Should().Throw<Exception>().WithMessage(message);
        }

        [Fact]
        public void MustBeAllowed_NotAllowedButExempt_DoesNotThrow()
        {
            // Arrange
            const bool isAllowed = false;

            // Act
            var act = () => isAllowed.MustBeAllowed(unless: () => true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MustBeAllowed_IsAllowed_DoesNotThrow()
        {
            // Arrange
            const bool isAllowed = true;

            // Act
            var act = () => isAllowed.MustBeAllowed();

            // Assert
            act.Should().NotThrow();
        }
    }
}
