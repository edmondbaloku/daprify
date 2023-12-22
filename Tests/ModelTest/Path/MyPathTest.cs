using CLI.Models;
using CLITests.Assert;
using FluentAssertions;

namespace CLITests.Paths
{
    [TestClass]
    public class TryMyPath
    {
        [TestMethod]
        public void Expect_Constructor_WithValidPath_ShouldSetPath()
        {
            // Arrange
            string validPath = "C:\\test";
            MyPath expectedResult = new(validPath);

            // Act
            MyPath sut = new(validPath);

            // Assert
            Asserts.VerifyString(sut, expectedResult);
        }

        [TestMethod]
        public void Expect_Constructor_WithNullPath_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => new MyPath(null!);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Expect_SetPath_WithValidPath_ShouldUpdatePath()
        {
            // Arrange
            MyPath sut = new("C:\\initial");
            string newPath = "C:\\new";
            MyPath expectedResult = new(newPath);

            // Act
            sut.SetPath(newPath);

            // Assert
            Asserts.VerifyString(sut, expectedResult);
        }

        [TestMethod]
        public void Expect_SetPath_WithNullPath_ShouldThrowArgumentNullException()
        {
            // Arrange
            MyPath myPath = new("C:\\initial");

            // Act & Assert
            Action act = () => myPath.SetPath(null!);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Expect_FromStringList_WithValidList_ShouldReturnMyPathList()
        {
            // Arrange
            List<string> paths = ["C:\\path1", "C:\\path2"];

            // Act
            List<MyPath> sut = MyPath.FromStringList(paths).ToList();

            // Assert
            IEnumerable<MyPath> expectedPaths = paths.Select(p => new MyPath(p));
            IEnumerable<MyPath> actualPaths = sut.Select(mp => mp);

            Asserts.VerifyEnumerableString(actualPaths, expectedPaths);
        }

        [TestMethod]
        public void Expect_ToString_WhenCalled_ShouldReturnPath()
        {
            // Arrange
            string path = "C:\\path";
            MyPath myPath = new(path);

            // Act
            string sut = myPath.ToString();

            // Assert
            sut.Should().Be(path);
        }
    }
}