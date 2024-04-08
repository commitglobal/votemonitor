using Vote.Monitor.TestUtils;

namespace Feature.MonitoringObservers.UnitTests.ValidatorTests;

public class TagValidatorTests
{
    private readonly Tag.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Tag.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_MonitoringNgoId_Empty()
    {
        // Arrange
        var request = new Tag.Request { MonitoringNgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonitoringNgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_MonitoringObserverIds_Empty()
    {
        // Arrange
        var request = new Tag.Request { MonitoringObserverIds = [] };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonitoringObserverIds);
    }

    [Fact]
    public void Validation_ShouldFail_When_MonitoringObserverIds_Contain_Empty()
    {
        // Arrange
        var request = new Tag.Request
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
    public void Validation_ShouldFail_When_Tags_Empty()
    {
        // Arrange
        var request = new Tag.Request { Tags = [] };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonitoringObserverIds);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Tags_Contain_Empty(string emptyTag)
    {
        // Arrange
        var request = new Tag.Request
        {
            Tags = [
                "a tag",
                emptyTag,
            ]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Tag.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            MonitoringNgoId = Guid.NewGuid(),
            MonitoringObserverIds = [
                Guid.NewGuid(),
            ],
            Tags = ["a tag"]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
