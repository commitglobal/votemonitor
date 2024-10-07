using Feature.PollingStation.Information.Upsert;

namespace Feature.PollingStation.Information.UnitTests.ValidatorTests;

public class UpsertValidatorTests
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
    public void Validation_ShouldFail_When_Answers_Invalid()
    {
        // Arrange
        var request = new Request
        {
            Answers = [
                new MultiSelectAnswerRequest(),
                new SingleSelectAnswerRequest(),
                new RatingAnswerRequest(),
                new DateAnswerRequest(),
                new TextAnswerRequest(),
                new NumberAnswerRequest()]
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
    public void Validation_ShouldFail_When_ObservationBreaks_ContainsIvalid()
    {
        // Arrange
        var request = new Request { Breaks = [
            new Request.BreakRequest()
            {
                Start = DateTime.UtcNow.AddDays(-1),
                End = DateTime.UtcNow,
            },
            new Request.BreakRequest()
            {
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(-1),
            }
        ]};

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Breaks[1]");
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validation_ShouldPass_When_AnswersEmpty()
    {
        // Arrange
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Answers = []
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validation_ShouldPass_When_AnswersNull()
    {
        // Arrange
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Answers = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

}
