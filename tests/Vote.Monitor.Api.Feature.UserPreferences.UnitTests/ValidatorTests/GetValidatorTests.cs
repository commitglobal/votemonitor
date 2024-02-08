
using Vote.Monitor.Api.Feature.UserPreferences.Get;
using FluentValidation.TestHelper;

namespace Vote.Monitor.Api.Feature.UserPreferences.UnitTests.ValidatorTests;
public class GetValidatorTests
{
    private readonly Validator _validator;

    public GetValidatorTests()
    {
        _validator = new Validator();
    }

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
