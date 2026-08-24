using Feature.UserPreferences.Update;
using FluentValidation.TestHelper;

namespace Feature.UserPreferences.UnitTests.ValidatorTests;
public class UpdateValidatorTests
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
    public void ShouldHaveErrorWhenPreferencesIsNull()
    {
        //arrange
        var model = new Request { Id = Guid.NewGuid(), Preferences = null! };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldHaveValidationErrorFor(x => x.Preferences);
    }

    [Fact]
    public void ShouldNotHaveErrors_WhenValidRequest()
    {
        var model = new Request
        {
            Id = Guid.NewGuid(),
            Preferences = new Dictionary<string, string> { ["languageCode"] = "EN" }
        };
        //act
        var result = _validator.TestValidate(model);
        //assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
