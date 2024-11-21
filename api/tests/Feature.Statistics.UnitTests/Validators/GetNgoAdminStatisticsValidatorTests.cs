namespace Feature.Statistics.UnitTests.Validators;

public class GetNgoAdminStatisticsValidatorTests
{
    private readonly GetNgoAdminStatistics.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new GetNgoAdminStatistics.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        // Arrange
        var request = new GetNgoAdminStatistics.Request { NgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_DataSource_Empty()
    {
        // Arrange
        var request = new GetNgoAdminStatistics.Request { DataSource = null! };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DataSource);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new GetNgoAdminStatistics.Request { ElectionRoundId = Guid.NewGuid(), NgoId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
