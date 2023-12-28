using CLI.Models;
using CLI.Validation;
using CLITests.Assert;
using FluentValidation.Results;

namespace ValidatorTest
{
    [TestClass]
    public class TryMyPathValidator
    {
        private readonly MyPathValidator _validator = new();

        [TestMethod]
        public void Expect_ValidPath_Give_No_Validation_Errors()
        {
            // Arrange
            MyPath validPath = new(Directory.GetCurrentDirectory());
            IEnumerable<MyPath> paths = [validPath];

            // Act
            ValidationResult result = _validator.Validate(paths);

            // Assert
            Asserts.VerifyTrue(result.IsValid);
        }

        [TestMethod]
        public void Expect_ValidPaths_Give_No_Validation_Errors()
        {
            // Arrange
            MyPath currentDir = new(Directory.GetCurrentDirectory());
            MyPath parentDir = new(Directory.GetParent(currentDir.ToString())!.FullName);
            IEnumerable<MyPath> paths = [currentDir, parentDir];

            // Act
            ValidationResult result = _validator.Validate(paths);

            // Assert
            Asserts.VerifyTrue(result.IsValid);
        }

        [TestMethod]
        public void Expect_InValidPath_Gives_Validation_Errors()
        {
            // Arrange
            MyPath invalidPath = new("nonexistent/path");
            IEnumerable<MyPath> paths = [invalidPath];

            // Act
            ValidationResult result = _validator.Validate(paths);

            // Assert
            Asserts.VerifyFalse(result.IsValid);
        }

        [TestMethod]
        public void Expect_InValidPaths_Gives_Validation_Errors()
        {
            // Arrange
            MyPath invalidPath1 = new("nonexistent/path");
            MyPath invalidPath2 = new("nonexistent/path/path2");
            IEnumerable<MyPath> paths = [invalidPath1, invalidPath2];

            // Act
            ValidationResult result = _validator.Validate(paths);

            // Assert
            Asserts.VerifyFalse(result.IsValid);
        }

        [TestMethod]
        public void Expect_InValidPath_Has_Single_ErrorMessage()
        {
            // Arrange
            MyPath invalidPath = new("nonexistent/path");
            IEnumerable<MyPath> paths = [invalidPath];

            // Act
            ValidationResult result = _validator.Validate(paths);

            // Assert
            Asserts.VerifyContainSingle(result.Errors);
            Asserts.VerifyContain(result.Errors[0].ErrorMessage, "Could not find the specified path");
            Asserts.VerifyContain(result.Errors[0].ErrorMessage, invalidPath.ToString());
        }

        [TestMethod]
        public void Expect_InValidPaths_Has_Single_ErrorMessage()
        {
            // Arrange
            MyPath invalidPath1 = new("nonexistent/path");
            MyPath invalidPath2 = new("nonexistent/path/path2");
            IEnumerable<MyPath> paths = [invalidPath1, invalidPath2];

            // Act
            ValidationResult result = _validator.Validate(paths);

            // Assert
            Asserts.VerifyHaveCount(result.Errors, paths.Count());
            Asserts.VerifyContain(result.Errors[0].ErrorMessage, "Could not find the specified path");
            Asserts.VerifyContain(result.Errors[0].ErrorMessage, invalidPath1.ToString());
            Asserts.VerifyContain(result.Errors[1].ErrorMessage, "Could not find the specified path");
            Asserts.VerifyContain(result.Errors[1].ErrorMessage, invalidPath2.ToString());
        }
    }
}