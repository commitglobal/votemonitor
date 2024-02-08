
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
    public void ShouldHaveErrorWhenLanguageIsoIsEmptyAndLanguageIdIsNull()
    {
        //arrange
        var model = new Request { Id = Guid.NewGuid(), LanguageId = null, LanguageIso = string.Empty };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldHaveValidationErrorFor(x => x.LanguageIso);
        result.ShouldHaveValidationErrorFor(x => x.LanguageId);
    }

    [Fact]
    public void ShouldHaveErrorWhenLanguageIsoIsNullAndLanguageIdIsNull()
    {
        //arrange
        var model = new Request { Id = Guid.NewGuid(), LanguageId = null, LanguageIso = null };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldHaveValidationErrorFor(x => x.LanguageIso);
        result.ShouldHaveValidationErrorFor(x => x.LanguageId);
    }



    [Fact]
    public void ShouldNotHaveErrorWhenLanguageIdIsSpecifiedLanguageIsoIsEmpty()
    {
        //arrange
        var model = new Request { Id = Guid.NewGuid(), LanguageId = Guid.NewGuid(), LanguageIso = string.Empty };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldNotHaveErrorWhenLanguageIdIsSpecifiedLanguageIsoIsNull()
    {
        var model = new Request { Id = Guid.NewGuid(), LanguageId = Guid.NewGuid(), LanguageIso = null };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldNotHaveAnyValidationErrors();
    }


    [Fact]
    public void ShouldNotHaveErrorWhenLanguageIsoIsSpecifiedAndLanguageIdIsNull()
    {
        //arrange
        var model = new Request { Id = Guid.NewGuid(), LanguageId = null, LanguageIso = "dd" };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldNotHaveAnyValidationErrors();
    }


    [Fact]
    public void ShouldNotHaveErrorWhenLanguageIsoIsSpecifiedAndLanguageIdoIsSpecified()
    {
        //arrange
        var model = new Request { Id = Guid.NewGuid(), LanguageId = Guid.NewGuid(), LanguageIso = "dd" };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
