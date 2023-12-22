using CLI.Models;
using CLITests.Assert;
namespace CLITests.Paths
{
    [TestClass]
    public class TryRelativePath
    {
        [TestMethod]
        public void Expect_Constructor_WithUnixPaths_ShouldCreateCorrectRelativePath()
        {
            // Arrange
            MyPath basePath = new("/home/user/Folder1");
            MyPath targetPath = new("/home/user/Folder1/Folder2");
            MyPath expected = new("Folder2");

            // Act
            RelativePath sut = new(basePath, targetPath);

            // Assert
            Asserts.VerifyString(sut, expected);
            Asserts.VerifyString(sut.TargetPath, targetPath);
        }

        [TestMethod]
        public void Expect_Constructor_WithNullBasePath_ShouldThrowArgumentNullException()
        {
            // Arrange
            MyPath basePath = null!;
            MyPath targetPath = new("/home/user/Folder1/Folder2");

            // Act & Assert
            Action act = () => new RelativePath(basePath, targetPath);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Expect_Constructor_WithNullTargetPath_ShouldThrowArgumentNullException()
        {
            // Arrange
            MyPath basePath = new("/home/user/Folder1");
            MyPath targetPath = null!;

            // Act & Assert
            Action act = () => new RelativePath(basePath, targetPath);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Expect_Constructor_WithUnixPathsToItself_ShouldReturnEmptyPath()
        {
            // Arrange
            MyPath basePath = new("/home/user/Folder1");
            MyPath expected = new(".");

            // Act
            RelativePath sut = new(basePath, basePath);

            // Assert
            Asserts.VerifyString(sut, expected);
        }
    }
}