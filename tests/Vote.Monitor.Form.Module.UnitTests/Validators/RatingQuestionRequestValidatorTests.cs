using FluentValidation.TestHelper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.Form.Module.Validators;

namespace Vote.Monitor.Form.Module.UnitTests.Validators;

public class RatingQuestionRequestValidatorTests
{
    private readonly RatingQuestionRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Fact]
    public void Validation_ShouldFail_When_EmptyId()
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [MemberData(nameof(Validators.ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(Validators.ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyText(TranslatedString invalidText)
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            Text = invalidText
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [MemberData(nameof(Validators.ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(Validators.ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyHelptext(TranslatedString invalidHelptext)
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            Helptext = invalidHelptext
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Helptext);
    }

    [Fact]
    public void Validation_ShouldPass_When_NoHelptext()
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            Helptext = null
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Helptext);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_CodeEmpty(string code)
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("'Code' must not be empty.");
    }

    [Theory]
    [MemberData(nameof(Validators.ValidatorsTestData.InvalidCodeTestCases), MemberType = typeof(Validators.ValidatorsTestData))]
    public void Validation_ShouldFail_When_CodeHasInvalidLength(string code)
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validation_ShouldFail_When_EmptyScale()
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest();

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Scale);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            Scale = RatingScaleModel.OneTo10,
            Helptext = Validators.ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = Validators.ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Code = "A code",
            Id = Guid.NewGuid()
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Scale);
    }
}
