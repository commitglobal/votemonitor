using FluentValidation.TestHelper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.Form.Module.Validators;

namespace Vote.Monitor.Form.Module.UnitTests.ValidatorTests;

public class SectionRequestValidatorTests
{
    private readonly SectionRequestValidator _sut = new(
    [
        LanguagesList.EN.Iso1,
        LanguagesList.RO.Iso1
    ]);

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidCodeTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_CodeInvalid(string code)
    {
        // Arrange
        var request = new SectionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_NameInvalid(TranslatedString invalidTitle)
    {
        // Arrange
        var request = new SectionRequest
        {
            Title = invalidTitle
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [MemberData(nameof(InvalidQuestionTestCases))]
    public void Validation_ShouldFail_When_InvalidQuestion(BaseQuestionRequest invalidQuestion)
    {
        // Arrange
        var validQuestion = new DateQuestionRequest
        {
            Code = "A",
            Helptext = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.First()
        };

        var request = new SectionRequest
        {
            Title = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Questions = [
                validQuestion,
                invalidQuestion
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validation_ShouldPass_When_RequestValid()
    {
        // Arrange
        var request = new SectionRequest
        {
            Title = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Code = "A code",
            Questions = []
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    public static IEnumerable<object[]> InvalidQuestionTestCases =>
        new List<object[]>
    {
        new object[] {
            new DateQuestionRequest()
        },
        new object[] {
            new MultiSelectQuestionRequest()
        },
        new object[] {
            new NumberQuestionRequest()
        },
        new object[] {
            new RatingQuestionRequest()
        },
        new object[] {
            new SingleSelectQuestionRequest()
        },
        new object[] {
            new TextQuestionRequest()
        }
    };
}
