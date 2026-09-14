using Feature.IncidentReports.Comments.Delete;

namespace Feature.IncidentReports.Comments.UnitTests.ValidatorTests;

public class DeleteRequestValidatorTests
{
    private readonly Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        var request = new Request { ElectionRoundId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_UserId_Empty()
    {
        var request = new Request { UserId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validation_ShouldFail_When_IncidentReportId_Empty()
    {
        var request = new Request { IncidentReportId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.IncidentReportId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        var request = new Request { Id = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IncidentReportId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
