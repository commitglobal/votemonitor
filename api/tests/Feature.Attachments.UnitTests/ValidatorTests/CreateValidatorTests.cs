using Feature.Attachments.Create;
using Vote.Monitor.TestUtils.Fakes;

namespace Feature.Attachments.UnitTests.ValidatorTests;

public class CreateValidatorTests
{
    private readonly Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ObserverId_Empty()
    {
        // Arrange
        var request = new Request { ObserverId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ObserverId);
    }

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_PollingStationId_Empty()
    {
        // Arrange
        var request = new Request { PollingStationId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PollingStationId);
    }

    [Fact]
    public void Validation_ShouldFail_When_FormId_Empty()
    {
        // Arrange
        var request = new Request { FormId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FormId);
    }

    [Fact]
    public void Validation_ShouldFail_When_QuestionId_Empty()
    {
        // Arrange
        var request = new Request { QuestionId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.QuestionId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Attachment_Empty()
    {
        // Arrange
        var request = new Request();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Attachment);
    }

    [Fact]
    public void Validation_ShouldFail_When_Attachment_ExceedsUploadLimit()
    {
        // Arrange
        var request = new Request
        {
            Attachment = FakeFormFile.New().HavingFileName("image.jpg").HavingLength(512 * 1024 * 1024 + 1).Please()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Attachment);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            FormId = Guid.NewGuid(),
            QuestionId = Guid.NewGuid(),
            Attachment = FakeFormFile.New().HavingFileName("image.jpg").HavingLength(256).Please()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

}
