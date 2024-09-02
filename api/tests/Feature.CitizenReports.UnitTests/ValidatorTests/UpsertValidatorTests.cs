using Vote.Monitor.TestUtils;
using Vote.Monitor.TestUtils.Utils;

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
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Upsert.Request
        {
            FormId = Guid.NewGuid(),
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
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
            Answers = []
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldPass_When_Email_Empty(string emptyString)
    {
        // Arrange
        var request = new Upsert.Request
        {
            FormId = Guid.NewGuid(),
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
            Answers = [],
            Email = emptyString
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldPass_When_ContactInformation_Empty(string emptyString)
    {
        // Arrange
        var request = new Upsert.Request
        {
            FormId = Guid.NewGuid(),
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
            Answers = [],
            ContactInformation = emptyString
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validation_ShouldFail_When_Email_Invalid()
    {
        // Arrange
        var request = new Upsert.Request
        {
            Email = "not an email"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x=>x.Email);
    }
    
    [Fact]
    public void Validation_ShouldFail_When_ContactInformation_ExceedsLimit()
    {
        // Arrange
        var request = new Upsert.Request
        {
            ContactInformation = "a".Repeat(2049)
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x=>x.ContactInformation);
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
            Answers = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
