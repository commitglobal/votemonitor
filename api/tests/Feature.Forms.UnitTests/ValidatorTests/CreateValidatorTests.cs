using Vote.Monitor.Core.Constants;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.TestUtils.Utils;

namespace Feature.Forms.UnitTests.ValidatorTests;

public class CreateValidatorTests
{
    private readonly Create.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Create.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        // Arrange
        var request = new Create.Request { NgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    [MemberData(nameof(TranslatedStringTestData.InvalidCodeTestCases), MemberType = typeof(TranslatedStringTestData))]
    public void Validation_ShouldFail_When_InvalidCode(string code)
    {
        // Arrange
        var request = new Create.Request
        {
            Code = code
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Theory]
    [MemberData(nameof(TranslatedStringTestData.InvalidPartiallyTranslatedTestCases),
        MemberType = typeof(TranslatedStringTestData))]
    public void Validation_ShouldFail_When_NameInvalid(TranslatedString invalidName)
    {
        // Arrange
        var request = new Create.Request
        {
            Name = invalidName,
            Languages =
            [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ]
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Name);
    }

    public static IEnumerable<object[]> InvalidPartiallyTranslatedFormDescriptionsTestCases =>
        InvalidPartiallyTranslatedDescriptionTestData
            .Select(x => new object[] { x })
            .ToList();

    private static IEnumerable<TranslatedString> InvalidPartiallyTranslatedDescriptionTestData =>
    [
        new()
        {
            [LanguagesList.IT.Iso1] = "an italian string"
        },
        new()
        {
            [LanguagesList.RO.Iso1] = "a long string",
            [LanguagesList.EN.Iso1] = "a".Repeat(10_001)
        },
        new() { [""] = "an empty" },
        new() { ["aaa"] = "an invalid iso" },
        new() { ["a"] = "an invalid iso" },
        new()
    ];

    [Theory]
    [MemberData(nameof(InvalidPartiallyTranslatedFormDescriptionsTestCases))]
    public void Validation_ShouldFail_When_DescriptionInvalid(TranslatedString invalidDescription)
    {
        // Arrange
        var request = new Create.Request
        {
            Description = invalidDescription,
            Languages =
            [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ]
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validation_ShouldFail_When_FormTypeEmpty()
    {
        // Arrange
        var request = new Create.Request();

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.FormType);
    }

    [Fact]
    public void Validation_ShouldFail_When_EmptySupportedLanguages()
    {
        // Arrange
        var request = new Create.Request
        {
            Languages = []
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Languages);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    [InlineData("UnknownIso")]
    public void Validation_ShouldFail_When_InvalidLanguageCodes(string invalidLanguageCode)
    {
        // Arrange
        var request = new Create.Request
        {
            Languages = [LanguagesList.RO.Iso1, invalidLanguageCode]
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Languages);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    [InlineData("UnknownIso")]
    public void Validation_ShouldFail_When_InvalidDefaultLanguage(string invalidLanguageCode)
    {
        // Arrange
        var request = new Create.Request
        {
            DefaultLanguage = invalidLanguageCode
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.DefaultLanguage);
    }

    [Fact]
    public void Validation_ShouldFail_When_DefaultLanguageNotInLanguageList()
    {
        // Arrange
        var request = new Create.Request
        {
            DefaultLanguage = LanguagesList.EN.Iso1,
            Languages = [LanguagesList.RO.Iso1]
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.DefaultLanguage)
            .WithErrorMessage("Languages should contain declared default language.");
    }


    [Fact]
    public void Validation_ShouldFail_When_InvalidQuestion()
    {
        // Arrange
        var request = new Create.Request
        {
            Languages =
            [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ],
            Questions =
            [
                new TextQuestionRequest
                {
                    Text = TranslatedStringTestData.ValidPartiallyTranslatedTestData.First()
                },
                new TextQuestionRequest
                {
                    Text = TranslatedStringTestData.InvalidPartiallyTranslatedTestData.First()
                }
            ]
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("Questions[1].Text[\"IT\"]");
    }

    [Fact]
    public void Validation_ShouldPass_When_EmptyQuestions()
    {
        // Arrange
        var request = new Create.Request
        {
            Questions = []
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Questions);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Languages = [LanguagesList.EN.Iso1, LanguagesList.RO.Iso1],
            DefaultLanguage = LanguagesList.EN.Iso1,
            Code = "A code",
            FormType = FormType.ClosingAndCounting,
            Name = TranslatedStringTestData.ValidPartiallyTranslatedTestData.First(),
            Description = TranslatedStringTestData.ValidPartiallyTranslatedTestData.First()
        };

        // Act
        var validationResult = _validator.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}