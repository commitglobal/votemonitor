namespace Feature.CitizenReports.UnitTests.ValidatorTests;

public class UpsertValidatorTests
{
    private readonly Upsert.Validator _validator = new();


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
    public void Validation_ShouldFail_When_PollingStationId_Empty()
    {
        // Arrange
        var request = new Upsert.Request { CitizenReportId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CitizenReportId);
    }

    [Fact]
    public void Validation_ShouldFail_When_LocationId_Empty()
    {
        // Arrange
        var request = new Upsert.Request { LocationId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LocationId);
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
            CitizenReportId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
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
            CitizenReportId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
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
            CitizenReportId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            Answers = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}