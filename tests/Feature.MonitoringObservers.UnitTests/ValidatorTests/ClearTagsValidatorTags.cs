using FluentValidation.TestHelper;

namespace Feature.MonitoringObservers.UnitTests.ValidatorTests;

public class ClearTagsValidatorTags
{
    private readonly ClearTags.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new ClearTags.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_MonitoringNgoId_Empty()
    {
        // Arrange
        var request = new ClearTags.Request { MonitoringNgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonitoringNgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_MonitoringObserverIds_Empty()
    {
        // Arrange
        var request = new ClearTags.Request { MonitoringObserverIds = [] };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonitoringObserverIds);
    }

    [Fact]
    public void Validation_ShouldFail_When_MonitoringObserverIds_Contain_Empty()
    {
        // Arrange
        var request = new ClearTags.Request
        {
            MonitoringObserverIds = [
                Guid.NewGuid(),
                Guid.Empty,
            ]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonitoringObserverIds);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new ClearTags.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringObserverIds = [
                Guid.NewGuid(),
            ]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
