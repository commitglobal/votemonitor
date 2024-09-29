namespace Feature.IssueReports.Attachments.UnitTests.ValidatorTests;

public class ListRequestValidatorTests
{
    private readonly List.Validator _validator = new();
    
    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new List.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_IssueReportId_Empty()
    {
        // Arrange
        var request = new List.Request { IssueReportId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IssueReportId);
    }

    [Fact]
    public void Validation_ShouldFail_When_FormId_Empty()
    {
        // Arrange
        var request = new List.Request { FormId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FormId);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            IssueReportId = Guid.NewGuid(),
            FormId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
