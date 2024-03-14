using FluentValidation.TestHelper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.Form.Module.Validators;

namespace Vote.Monitor.Form.Module.UnitTests.ValidatorTests;

public class SelectOptionRequestValidatorTests
{
    private readonly SelectOptionRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Fact]
    public void Validation_ShouldFail_When_EmptyId()
    {
        // Arrange
        var selectOptionRequest = new SelectOptionRequest
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(selectOptionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_TextInvalid(TranslatedString invalidText)
    {
        // Arrange
        var selectOptionRequest = new SelectOptionRequest
        {
            Text = invalidText
        };

        // Act
        var validationResult = _sut.TestValidate(selectOptionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.ValidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldPass_When_TextValid(TranslatedString validText)
    {
        // Arrange
        var selectOptionRequest = new SelectOptionRequest
        {
            Text = validText
        };

        // Act
        var validationResult = _sut.TestValidate(selectOptionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Text);
    }
}
