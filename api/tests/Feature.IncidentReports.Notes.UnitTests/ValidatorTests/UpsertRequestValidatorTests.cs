using Feature.IncidentsReports.Notes.Upsert;
using Vote.Monitor.TestUtils.Utils;

namespace Feature.IncidentReports.Notes.UnitTests.ValidatorTests;

public class UpsertRequestValidatorTests
{
    private readonly Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
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
    public void Validation_ShouldFail_When_IncidentReportId_Empty()
    {
        // Arrange
        var request = new Request { IncidentReportId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IncidentReportId);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Text_Empty(string text)
    {
        // Arrange
        var request = new Request { Text = text };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validation_ShouldFail_When_Text_ExceedsLimits()
    {
        // Arrange
        var request = new Request { Text = "a".Repeat(10_001) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Text);
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
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IncidentReportId = Guid.NewGuid(),
            QuestionId = Guid.NewGuid(),
            FormId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            Text = "some text"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}