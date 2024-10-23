using Feature.CitizenReports.Requests;
using Feature.CitizenReports.Validators;

namespace Feature.CitizenReports.UnitTests.ValidatorTests;

public class CitizenReportsAggregateFilterValidatorTests
{
    private readonly CitizenReportsAggregateFilterValidator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        // Arrange
        var request = new CitizenReportsAggregateFilter { NgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new CitizenReportsAggregateFilter { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new CitizenReportsAggregateFilter
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}