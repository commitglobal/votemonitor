using FluentValidation.TestHelper;

namespace Feature.ObserverGuide.UnitTests.Validators;

public class GetByIdValidatorTests
{
    private readonly GetById.Validator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        // Arrange
        var model = new GetById.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var model = new GetById.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }


    [Fact]
    public void Should_Not_Have_Error_When_Valid_Request()
    {
        // Arrange
        var model = new GetById.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}