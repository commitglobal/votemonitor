namespace Feature.QuickReports.UnitTests.ValidatorTests;

public class GetAggregatedValidatorTests
{
    private readonly GetAggregated.Validator _validator = new();

    private static GetAggregated.Request ValidRequest() => new()
    {
        ElectionRoundId = Guid.NewGuid(),
        NgoId = Guid.NewGuid()
    };

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        // Arrange
        var request = ValidRequest();
        request.ElectionRoundId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Should_Have_Error_When_NgoId_Is_Empty()
    {
        // Arrange
        var request = ValidRequest();
        request.NgoId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Should_Have_Error_When_DataSource_Is_Empty()
    {
        // Arrange
        var request = ValidRequest();
        request.DataSource = null!;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DataSource);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        // Arrange
        var request = ValidRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
