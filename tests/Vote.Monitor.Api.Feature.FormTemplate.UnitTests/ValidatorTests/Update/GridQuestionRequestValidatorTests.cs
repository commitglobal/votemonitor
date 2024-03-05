using FluentValidation.TestHelper;

namespace Vote.Monitor.Api.Feature.FormTemplate.UnitTests.ValidatorTests.Update;

public class GridQuestionRequestValidatorTests
{
    private readonly GridQuestionRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyText(TranslatedString invalidText)
    {
        // Arrange
        var gridQuestionRequest = new GridQuestionRequest
        {
            Text = invalidText
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyHelptext(TranslatedString invalidHelptext)
    {
        // Arrange
        var gridQuestionRequest = new GridQuestionRequest
        {
            Helptext = invalidHelptext
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Helptext);
    }

    [Fact]
    public void Validation_ShouldPass_When_NoHelptext()
    {
        // Arrange
        var gridQuestionRequest = new GridQuestionRequest
        {
            Helptext = null
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Helptext);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_ScalePlaceholderEmpty(TranslatedString scalePlaceholder)
    {
        // Arrange
        var gridQuestionRequest = new GridQuestionRequest
        {
            ScalePlaceholder = scalePlaceholder
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.ScalePlaceholder);
    }

    [Fact]
    public void Validation_ShouldPass_When_EmptyRows()
    {
        // Arrange
        var gridQuestionRequest = new GridQuestionRequest();

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Rows);
    }
    [Fact]
    public void Validation_ShouldFail_When_InvalidRow()
    {
        // Arrange
        var gridQuestionRequest = new GridQuestionRequest
        {
            Scale = RatingScale.OneTo10,
            ScalePlaceholder = ValidatorsTestData.ValidPartiallyTranslatedTestData.Last(),
            Rows = [
                new GridQuestionRowRequest
                {
                    Code = "A",
                    Helptext = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
                    Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.Last()
                },
                new GridQuestionRowRequest
                {
                    Code = "",
                    Helptext = ValidatorsTestData.InvalidPartiallyTranslatedTestData.First(),
                    Text = ValidatorsTestData.InvalidPartiallyTranslatedTestData.Last()
                }
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRequest);

        // Assert
        validationResult.ShouldHaveValidationErrorFor("Rows[1].Code");
        validationResult.ShouldHaveValidationErrorFor("Rows[1].Text");
        validationResult.ShouldHaveValidationErrorFor(@"Rows[1].Helptext[""IT""]");
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var gridQuestionRequest = new GridQuestionRequest
        {
            ScalePlaceholder = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Helptext = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Scale = RatingScale.OneTo10,
            Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            HasNotKnownColumn = true,
            Rows = [
                new GridQuestionRowRequest
                {
                    Id = Guid.NewGuid(),
                    Helptext = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
                    Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
                    Code = "a code"
                }
            ]
        };

        // Act
        var validationResult = _sut.TestValidate(gridQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
