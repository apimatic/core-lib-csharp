using APIMatic.Core.Request;
using APIMatic.Core.Test.MockTypes.Models;
using NUnit.Framework;

namespace APIMatic.Core.Test.Request
{
    [TestFixture]
    public class RequestBuilderExtensionsTest
    {
        [Test]
        public void UpdateBodyValueByPointer_ValidPointer_UpdatesValue()
        {
            // Arrange
            var person = new Person { Name = "Alice", Age = 30 };
            var pointer = "/age";

            // Act
            var updated = RequestBuilderExtensions.UpdateValueByPointer(
                person,
                pointer,
                old => int.Parse(old.ToString() ?? "") + 5
            );

            // Assert
            Assert.AreEqual(35, updated.Age);
            Assert.AreEqual("Alice", updated.Name);
        }

        [Test]
        public void UpdateBodyValueByPointer_NullValue_ReturnsOriginal()
        {
            // Act
            var result = RequestBuilderExtensions.UpdateValueByPointer<Person>(
                null,
                "/name",
                old => "Bob"
            );

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UpdateBodyValueByPointer_NullPointer_ReturnsOriginal()
        {
            var person = new Person { Name = "Charlie", Age = 40 };
            var result = RequestBuilderExtensions.UpdateValueByPointer(
                person,
                null,
                old => "David"
            );

            Assert.AreEqual("Charlie", result.Name);
        }

        [Test]
        public void UpdateBodyValueByPointer_NullUpdater_ReturnsOriginal()
        {
            var person = new Person { Name = "Charlie", Age = 40 };
            var result = RequestBuilderExtensions.UpdateValueByPointer(
                person,
                "/name",
                null
            );

            Assert.AreEqual("Charlie", result.Name);
        }

        [Test]
        public void UpdateBodyValueByPointer_UpdaterReturnsNull_ReturnsOriginal()
        {
            var person = new Person { Name = "Eva", Age = 50 };
            var pointer = "/name";

            var result = RequestBuilderExtensions.UpdateValueByPointer(
                person,
                pointer,
                old => null
            );

            Assert.AreEqual("Eva", result.Name);
        }

        [Test]
        public void UpdateBodyValueByPointer_InvalidPointer_ReturnsOriginal()
        {
            var person = new Person { Name = "Frank", Age = 60 };
            var invalidPointer = "/nonexistent";

            var result = RequestBuilderExtensions.UpdateValueByPointer(
                person,
                invalidPointer,
                old => "ShouldNotApply"
            );

            Assert.AreEqual("Frank", result.Name);
        }
    }
}
