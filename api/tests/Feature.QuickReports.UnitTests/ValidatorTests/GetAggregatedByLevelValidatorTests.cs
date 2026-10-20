using Vote.Monitor.Core.Models;

namespace Feature.QuickReports.UnitTests.ValidatorTests;

public class GetAggregatedByLevelValidatorTests
{
    private readonly GetAggregatedByLevel.Validator _validator = new();

    public static TheoryData<LocationLevel> Levels => new(LocationLevel.List);

    private static GetAggregatedByLevel.Request ValidRequest() => new()
    {
        ElectionRoundId = Guid.NewGuid(),
        NgoId = Guid.NewGuid(),
        Level = LocationLevel.Level1
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
    public void Should_Have_Error_When_Level_Is_Empty()
    {
        // Arrange
        var request = ValidRequest();
        request.Level = null!;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level);
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

    [Theory]
    [MemberData(nameof(Levels))]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid(LocationLevel level)
    {
        // Arrange
        var request = ValidRequest();
        request.Level = level;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
