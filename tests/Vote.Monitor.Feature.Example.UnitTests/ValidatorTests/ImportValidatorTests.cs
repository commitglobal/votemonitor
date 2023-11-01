using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.ValidatorTests;

public class ImportValidatorTests
{
    private readonly Import.Validator _validator = new ();
    private static IFormFile CreateMockFormFile(string fileName, long fileSizeInBits)
    {
        var formFile = Substitute.For<IFormFile>();

        formFile.FileName.Returns(fileName);
        formFile.Length.Returns(fileSizeInBits);

        return formFile;
    }

    [Fact]
    public void Validate_File_NotEmpty_ShouldPass()
    {
        // Arrange
        var request = new Import.Request { File = CreateMockFormFile("file.csv", 0) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validate_File_Empty_ShouldFail()
    {
        // Arrange
        var request = new Import.Request { File = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validate_FileSize_Valid_ShouldPass()
    {
        // Arrange
        
        var request = new Import.Request { File = new FormFile(Stream.Null, 0, 25 * 1024 * 1024 - 1, "File", "file.csv") };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validate_FileSize_ExceedsLimit_ShouldFail()
    {
        // Arrange
        
        var request = new Import.Request { File = new FormFile(Stream.Null, 0, 25 * 1024 * 1024 + 1, "File", "file.csv") };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("The selected file exceeds 25 MB limit.");
    }

    [Fact]
    public void Validate_FileExtension_Valid_ShouldPass()
    {
        // Arrange
        
        var request = new Import.Request { File = new FormFile(Stream.Null, 0, 0, "File", "file.csv") };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validate_FileExtension_Invalid_ShouldFail()
    {
        // Arrange
        
        var request = new Import.Request { File = new FormFile(Stream.Null, 0, 0, "File", "file.txt") };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("Only CSV files are accepted.");
    }
}
