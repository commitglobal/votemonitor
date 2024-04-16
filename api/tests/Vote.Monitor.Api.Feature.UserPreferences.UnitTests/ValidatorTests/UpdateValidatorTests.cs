
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
        var model = new Request { Id = Guid.NewGuid(), LanguageId = Guid.Empty };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldHaveValidationErrorFor(x => x.LanguageId);
    }

    [Fact]
    public void ShouldHaveErrorWhenLanguageIdIsSpecifiedButISNotFoundInLanguageList()
    {
        //arrange
        var model = new Request { Id = Guid.NewGuid(), LanguageId = Guid.NewGuid() };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldHaveValidationErrorFor(x => x.LanguageId);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenLanguageIdIsSpecifiedAndLanguageIdisFromList()
    {
        Guid languageId = new Guid("094b3769-68b1-6211-ba2d-6bba92d6a167");
        var model = new Request { Id = Guid.NewGuid(), LanguageId = languageId };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
