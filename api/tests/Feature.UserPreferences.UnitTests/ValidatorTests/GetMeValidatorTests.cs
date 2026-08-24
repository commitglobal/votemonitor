using Feature.UserPreferences.GetMe;
using FluentValidation.TestHelper;

namespace Feature.UserPreferences.UnitTests.ValidatorTests;

public class GetMeValidatorTests
{
    private readonly Validator _validator = new();

    [Fact]
    public void ShouldHaveErrorWhenIdIsEmpty()
    {
        //arrange
        var model = new Request();
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenIdIsSpecified()
    {
        //arrange
        var model = new Request { Id = Guid.NewGuid() };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
