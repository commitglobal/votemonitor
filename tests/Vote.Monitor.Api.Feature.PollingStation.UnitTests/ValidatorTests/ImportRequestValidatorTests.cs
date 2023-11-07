namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.ValidatorTests;

public class ImportRequestValidatorTests
{
    private readonly Import.Validator _validator = new();
    private static IFormFile CreateMockFormFile(string fileName, long fileSizeInBits)
    {
        var formFile = Substitute.For<IFormFile>();

        formFile.FileName.Returns(fileName);
        formFile.Length.Returns(fileSizeInBits);

        return formFile;
    }

    [Fact]
    public void Validation_ShouldPass_When_File_NotEmpty()
    {
        // Arrange
        var request = new Import.Request { File = CreateMockFormFile("file.csv", 123) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validation_ShouldFail_When_File_Empty()
    {
        // Arrange
        var request = new Import.Request { File = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validation_ShouldPass_When_FileSize_Valid()
    {
        // Arrange

        var request = new Import.Request { File = CreateMockFormFile("file.csv", 25 * 1024 * 1024 - 1) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validation_ShouldFail_When_FileSize_ExceedsLimit()
    {
        // Arrange

        var request = new Import.Request { File = CreateMockFormFile("file.csv", 25 * 1024 * 1024 + 1) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("The selected file exceeds 25 MB limit.");
    }

    [Fact]
    public void Validation_ShouldPass_When_FileExtension_Valid()
    {
        // Arrange

        var request = new Import.Request { File = CreateMockFormFile("file.csv", 123) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validation_ShouldFail_When_FileExtension_Invalid()
    {
        // Arrange

        var request = new Import.Request { File = CreateMockFormFile("file.txt", 123) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("Only CSV files are accepted.");
    }
}
