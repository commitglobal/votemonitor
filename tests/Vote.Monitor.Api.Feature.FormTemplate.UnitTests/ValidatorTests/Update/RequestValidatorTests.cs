namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.ValidatorTests.Update;

public class RequestValidatorTests
{
    private readonly RequestValidator _sut = new();

    [Fact]
    public void Validation_ShouldFail_When_EmptyId()
    {
        // Arrange
        var request = new Request
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidId()
    {
        // Arrange
        var request = new Request
        {
            Id = Guid.NewGuid()
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Id);
    }


    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_CodeEmpty(string code)
    {
        // Arrange
        var request = new Request
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(request);

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
        var request = new Request
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validation_ShouldPass_When_CodeValid()
    {
        // Arrange
        var request = new Request
        {
            Code = "code"
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_NameInvalid(TranslatedString invalidName)
    {
        // Arrange
        var request = new Request
        {
            Name = invalidName,
            Languages = [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.ValidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldPass_When_NameValid(TranslatedString validName)
    {
        // Arrange
        var request = new Request
        {
            Name = validName,
            Languages = [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validation_ShouldFail_When_EmptySupportedLanguages()
    {
        // Arrange
        var request = new Request
        {
            Languages = []
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Languages);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_EmptyLanguageCodes(string invalidLanguageCode)
    {
        // Arrange
        var request = new Request
        {
            Languages = [LanguagesList.RO.Iso1, invalidLanguageCode]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Languages);
    }

    [Fact]
    public void Validation_ShouldFail_When_ValidSupportedLanguages()
    {
        // Arrange
        var request = new Request
        {
            Languages = [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Languages);
    }

    [Fact]
    public void Validation_ShouldFail_When_InvalidSection()
    {
        // Arrange
        var request = new Request
        {
            Languages = [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ],
            Sections = [
                new SectionRequest
                {
                    Title = ValidatorsTestData.ValidPartiallyTranslatedTestData.First()
                },
                new SectionRequest
                {
                    Title = ValidatorsTestData.InvalidPartiallyTranslatedTestData.First()
                },
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("Sections[1].Title[\"IT\"]");
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidFormSections()
    {
        // Arrange
        var request = new Request
        {
            Languages = [
                LanguagesList.EN.Iso1,
                LanguagesList.RO.Iso1
            ],
            Sections = [
                new SectionRequest
                {
                    Title = ValidatorsTestData.ValidPartiallyTranslatedTestData.First()
                },
                new SectionRequest
                {
                    Title = ValidatorsTestData.ValidPartiallyTranslatedTestData.First()
                },
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Sections);
    }

    [Fact]
    public void Validation_ShouldPass_When_EmptyFormSections()
    {
        // Arrange
        var request = new Request
        {
            Sections = []
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Sections);
    }

    [Fact]
    public void Validation_ShouldFail_When_EmptyFormType()
    {
        // Arrange
        var request = new Request();

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.FormType);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidFormType()
    {
        // Arrange
        var request = new Request
        {
            FormType = FormType.ClosingAndCounting
        };

        // Act
        var validationResult = _sut.TestValidate(request);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.FormType);
    }
}
