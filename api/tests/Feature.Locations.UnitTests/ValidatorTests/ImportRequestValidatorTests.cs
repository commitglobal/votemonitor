using Vote.Monitor.TestUtils.Fakes;

namespace Feature.Locations.UnitTests.ValidatorTests;

public class ImportRequestValidatorTests
{
    private readonly Import.Validator _validator = new();

    [Fact]
    public void Validation_ShouldPass_When_File_NotEmpty()
    {
        // Arrange
        var formFile = FakeFormFile.New()
            .HavingFileName("file.csv")
            .HavingLength(123)
            .Please();

        var request = new Import.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            File = formFile
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validation_ShouldFail_When_File_Empty()
    {
        // Arrange
        var request = new Import.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            File = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validation_ShouldPass_When_FileSize_Valid()
    {
        // Arrange
        var formFile = FakeFormFile.New()
            .HavingFileName("file.csv")
            .HavingLength(25 * 1024 * 1024 - 1)
            .Please();

        var request = new Import.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            File = formFile
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }


    [Fact]
    public void Validation_ShouldPass_When_FileExtension_Valid()
    {
        // Arrange
        var formFile = FakeFormFile.New()
            .HavingFileName("file.csv")
            .HavingLength(123)
            .Please();

        var request = new Import.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            File = formFile };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validation_ShouldFail_When_FileExtension_Invalid()
    {
        // Arrange
        var formFile = FakeFormFile.New()
            .HavingFileName("file.txt")
            .HavingLength(123)
            .Please();

        var request = new Import.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            File = formFile };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("Only CSV files are accepted.");
    }
}
