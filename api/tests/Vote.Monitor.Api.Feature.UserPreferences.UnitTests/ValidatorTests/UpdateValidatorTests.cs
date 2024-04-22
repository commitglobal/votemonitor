using Vote.Monitor.Api.Feature.UserPreferences.Update;
using FluentValidation.TestHelper;

namespace Vote.Monitor.Api.Feature.UserPreferences.UnitTests.ValidatorTests;
public class UpdateValidatorTests
{
    private readonly Validator _validator;

    public UpdateValidatorTests()
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
    public void ShouldHaveErrorWhenLanguageIdIsEmpty()
    {
        //arrange
        var model = new Request { Id = Guid.NewGuid(), LanguageCode = "" };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldHaveValidationErrorFor(x => x.LanguageCode);
    }

    [Fact]
    public void ShouldHaveErrorWhenLanguageUnknown()
    {
        //arrange
        var model = new Request { Id = Guid.NewGuid(), LanguageCode = "Unknown" };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldHaveValidationErrorFor(x => x.LanguageCode);
    }

    [Fact]
    public void ShouldNotHaveErrors_WhenValidRequest()
    {
        var model = new Request { Id = Guid.NewGuid(), LanguageCode = "EN" };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
