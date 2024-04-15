using Vote.Monitor.TestUtils.Fakes;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.UnitTests.ValidatorTests;

public class CreateValidatorTests
{
    private readonly Create.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ObserverId_Empty()
    {
        // Arrange
        var request = new Create.Request { ObserverId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ObserverId);
    }

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Create.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_PollingStationId_Empty()
    {
        // Arrange
        var request = new Create.Request { PollingStationId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PollingStationId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Attachment_Empty()
    {
        // Arrange
        var request = new Create.Request();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Attachment);
    }

    [Fact]
    public void Validation_ShouldFail_When_Attachment_ExceedsUploadLimit()
    {
        // Arrange
        var request = new Create.Request
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
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Attachment = FakeFormFile.New().HavingFileName("image.jpg").HavingLength(256).Please()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

}
