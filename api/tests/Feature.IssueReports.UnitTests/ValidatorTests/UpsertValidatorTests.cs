using FluentValidation.TestHelper;
using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.TestUtils;
using Vote.Monitor.TestUtils.Utils;
using Xunit;

namespace Feature.IssueReports.UnitTests.ValidatorTests;

public class UpsertValidatorTests
{
    private readonly Upsert.Validator _validator = new();

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

    [Theory]
    [MemberData(nameof(TestData.EmptyNullableGuidTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_PollingStationId_Empty_And_LocationType_PollingStation(
        Guid? pollingStationId)
    {
        // Arrange
        var request = new Upsert.Request
        {
            PollingStationId = pollingStationId,
            LocationType = IssueReportLocationType.PollingStation
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PollingStationId);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_LocationDescription_Empty_And_LocationType_OtherLocation(
        string? locationDescription)
    {
        // Arrange
        var request = new Upsert.Request
        {
            LocationDescription = locationDescription,
            LocationType = IssueReportLocationType.OtherLocation
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LocationDescription);
    }
    
    [Fact]
    public void Validation_ShouldFail_When_LocationDescription_ExceedsLimit_And_LocationType_OtherLocation()
    {
        // Arrange
        var request = new Upsert.Request
        {
            LocationDescription = "a".Repeat(1025),
            LocationType = IssueReportLocationType.OtherLocation
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LocationDescription);
    }
    
    [Fact]
    public void Validation_ShouldFail_When_FormId_Empty()
    {
        // Arrange
        var request = new Upsert.Request { FormId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FormId);
    }

    [Fact]
    public void Validation_ShouldFail_When_IssueReportId_Empty()
    {
        // Arrange
        var request = new Upsert.Request { IssueReportId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IssueReportId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Answers_Invalid()
    {
        // Arrange
        var request = new Upsert.Request
        {
            Answers =
            [
                new MultiSelectAnswerRequest(),
                new SingleSelectAnswerRequest(),
                new RatingAnswerRequest(),
                new DateAnswerRequest(),
                new TextAnswerRequest(),
                new NumberAnswerRequest()
            ]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Answers[0].QuestionId");
        result.ShouldHaveValidationErrorFor("Answers[1].QuestionId");
        result.ShouldHaveValidationErrorFor("Answers[2].QuestionId");
        result.ShouldHaveValidationErrorFor("Answers[3].QuestionId");
        result.ShouldHaveValidationErrorFor("Answers[4].QuestionId");
        result.ShouldHaveValidationErrorFor("Answers[5].QuestionId");
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Upsert.Request
        {
            FormId = Guid.NewGuid(),
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            IssueReportId = Guid.NewGuid(),
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validation_ShouldPass_When_Answers_Empty()
    {
        // Arrange
        var request = new Upsert.Request
        {
            FormId = Guid.NewGuid(),
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            IssueReportId = Guid.NewGuid(),
            LocationType = IssueReportLocationType.PollingStation,
            Answers = []
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validation_ShouldPass_When_Answers_Null()
    {
        // Arrange
        var request = new Upsert.Request
        {
            FormId = Guid.NewGuid(),
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            LocationType = IssueReportLocationType.PollingStation,
            IssueReportId = Guid.NewGuid(),
            Answers = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}