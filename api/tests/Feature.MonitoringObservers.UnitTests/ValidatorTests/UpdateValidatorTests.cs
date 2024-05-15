using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.TestUtils;

namespace Feature.MonitoringObservers.UnitTests.ValidatorTests;

public class UpdateValidatorTests
{
    private readonly Update.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Update.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_MonitoringNgoId_Empty()
    {
        // Arrange
        var request = new Update.Request { MonitoringNgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonitoringNgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new Update.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldFail_When_Status_Empty()
    {
        // Arrange
        var request = new Update.Request();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }


    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Tags_Contain_Empty(string emptyTag)
    {
        // Arrange
        var request = new Update.Request
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
        var request = new Update.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            MonitoringNgoId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            Status = MonitoringObserverStatus.Active,
            Tags = []
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
