using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.TestUtils.Utils;

namespace Feature.QuickReports.UnitTests.ValidatorTests;

public class UpsertRequestValidatorTests
{
    private readonly Upsert.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new Upsert.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldFail_When_ObserverId_Empty()
    {
        // Arrange
        var request = new Upsert.Request { ObserverId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ObserverId);
    }

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Upsert.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_QuickReportLocationType_Empty()
    {
        // Arrange
        var request = new Upsert.Request { QuickReportLocationType = default! };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.QuickReportLocationType);
    }

    [Fact]
    public void Validation_ShouldFail_When_Title_Empty()
    {
        // Arrange
        var request = new Upsert.Request { Title = default! };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validation_ShouldFail_When_Description_Empty()
    {
        // Arrange
        var request = new Upsert.Request { Description = default! };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validation_ShouldFail_When_Title_ExceedsLimit()
    {
        // Arrange
        var request = new Upsert.Request { Title = "a".Repeat(1_025) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validation_ShouldFail_When_Description_ExceedsLimit()
    {
        // Arrange
        var request = new Upsert.Request { Description = "a".Repeat(10_001) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyNullableGuidTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_QuickReportLocationType_VisitedPollingStation_And_PollingStationId_Empty(Guid? pollingStationId)
    {
        // Arrange
        var request = new Upsert.Request
        {
            PollingStationId = pollingStationId,
            QuickReportLocationType = QuickReportLocationType.VisitedPollingStation
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PollingStationId);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_QuickReportLocationType_OtherPollingStation_And_PollingStationDetails_Empty(string? pollingStationDetails)
    {
        // Arrange
        var request = new Upsert.Request
        {
            PollingStationDetails = pollingStationDetails,
            QuickReportLocationType = QuickReportLocationType.OtherPollingStation
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PollingStationDetails);
    }

    [Fact]
    public void Validation_ShouldFail_When_QuickReportLocationType_OtherPollingStation_And_PollingStationDetails_ExceedsLimit()
    {
        // Arrange
        var request = new Upsert.Request
        {
            PollingStationDetails = "a".Repeat(1_025),
            QuickReportLocationType = QuickReportLocationType.OtherPollingStation
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PollingStationDetails);
    }


    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Upsert.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            Title = "some title",
            Description = "some description",
            QuickReportLocationType = QuickReportLocationType.NotRelatedToAPollingStation
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
