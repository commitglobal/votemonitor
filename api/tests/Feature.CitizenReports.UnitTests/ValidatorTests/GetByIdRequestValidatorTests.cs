namespace Feature.CitizenReports.UnitTests.ValidatorTests;

public class GetByIdRequestValidatorTests
{
    private readonly GetById.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new GetById.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }


    [Fact]
    public void Validation_ShouldFail_When_CitizenReportId_Empty()
    {
        // Arrange
        var request = new GetById.Request { CitizenReportId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CitizenReportId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        // Arrange
        var request = new GetById.Request { NgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new GetById.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
            NgoId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
