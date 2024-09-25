using FluentValidation.TestHelper;

namespace Feature.ObserverGuide.UnitTests.Validators;

public class DeleteValidatorTests
{
    private readonly Delete.Validator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        var model = new Delete.Request { ElectionRoundId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        var model = new Delete.Request { Id = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid_Request()
    {
        var model = new Delete.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}