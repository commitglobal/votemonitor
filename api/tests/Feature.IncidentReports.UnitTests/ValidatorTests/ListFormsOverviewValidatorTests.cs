using Feature.IncidentReports.Requests;
using Feature.IncidentReports.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Feature.IncidentReports.UnitTests.ValidatorTests;

public class ListFormsOverviewValidatorTests
{
    private readonly IncidentReportsAggregateFilterValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        // Arrange
        var request = new IncidentReportsAggregateFilter
        {
            ElectionRoundId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Should_Have_Error_When_NgoId_Is_Empty()
    {
        // Arrange
        var request = new IncidentReportsAggregateFilter
        {
            NgoId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        // Arrange
        var request = new IncidentReportsAggregateFilter
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