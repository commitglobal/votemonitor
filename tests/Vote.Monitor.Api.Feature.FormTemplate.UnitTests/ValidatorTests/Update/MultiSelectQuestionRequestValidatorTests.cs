using Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;
using Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;

namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.ValidatorTests.Update;

public class MultiSelectQuestionRequestValidatorTests
{
    private readonly MultiSelectQuestionRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Fact]
    public void Validation_ShouldFail_When_IdEmpty()
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("'Code' must not be empty.");
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyText(TranslatedString invalidText)
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            Text = invalidText
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyHelptext(TranslatedString invalidHelptext)
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            Helptext = invalidHelptext
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Helptext);
    }

    [Fact]
    public void Validation_ShouldPass_When_NoHelptext()
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            Helptext = null
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Helptext);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_CodeEmpty(string code)
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("'Code' must not be empty.");
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidCodeTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_CodeHasInvalidLength(string code)
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validation_ShouldFail_When_SomeOptionsAreInvalid()
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            Options = [
                new SelectOptionRequest
                {
                    Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.Last()
                },
                new SelectOptionRequest
                {
                    Text = ValidatorsTestData.InvalidPartiallyTranslatedTestData.Last()
                }
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("Options[1].Text");
    }

    [Fact]
    public void Validation_ShouldPass_When_EmptyOptions()
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            Options = []
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Options);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {

            Id = Guid.NewGuid(),
            Helptext = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Code = "A code",
            Options = [
                new SelectOptionRequest
                {
                    Id = Guid.NewGuid(),
                    Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.Last()
                }
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
