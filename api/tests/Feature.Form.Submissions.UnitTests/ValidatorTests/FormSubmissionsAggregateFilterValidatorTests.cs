using Feature.Form.Submissions.Requests;
using Feature.Form.Submissions.Validators;

namespace Feature.Form.Submissions.UnitTests.ValidatorTests;

public class FormSubmissionsAggregateFilterValidatorTests
{
    private readonly FormSubmissionsAggregateFilterValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        // Arrange
        var request = new FormSubmissionsAggregateFilter
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
        var request = new FormSubmissionsAggregateFilter
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
        var request = new FormSubmissionsAggregateFilter
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            FormId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}