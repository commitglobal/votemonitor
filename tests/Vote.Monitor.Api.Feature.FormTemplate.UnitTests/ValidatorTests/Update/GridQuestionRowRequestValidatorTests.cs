using FluentValidation.TestHelper;

namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.ValidatorTests.Update;

public class GridQuestionRowRequestValidatorTests
{
    private readonly GridQuestionRowRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Fact]
    public void Validation_ShouldFail_When_IdEmpty()
    {
        // Arrange
        var gridQuestionRowRequest = new GridQuestionRowRequest
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRowRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_CodeEmpty(string code)
    {
        // Arrange
        var gridQuestionRowRequest = new GridQuestionRowRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRowRequest);

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
        var gridQuestionRowRequest = new GridQuestionRowRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRowRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_InvalidText(TranslatedString text)
    {
        // Arrange
        var gridQuestionRowRequest = new GridQuestionRowRequest
        {
            Text = text
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRowRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_InvalidHelptext(TranslatedString helptext)
    {
        // Arrange
        var gridQuestionRowRequest = new GridQuestionRowRequest
        {
            Helptext = helptext
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRowRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Helptext);
    }

    [Fact]
    public void Validation_ShouldPass_When_NoHelptext()
    {
        // Arrange
        var gridQuestionRowRequest = new GridQuestionRowRequest
        {
            Helptext = null
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRowRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Helptext);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var gridQuestionRowRequest = new GridQuestionRowRequest
        {
            Code = "code",
            Helptext = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Id = Guid.NewGuid()
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRowRequest);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
