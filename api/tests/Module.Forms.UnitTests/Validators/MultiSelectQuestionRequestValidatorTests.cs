using FluentAssertions;
using FluentValidation.TestHelper;
using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Module.Forms.Requests;
using Module.Forms.Validators;

namespace Module.Forms.UnitTests.Validators;

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
    public void Validation_ShouldFail_When_InvalidDisplayLogic()
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            DisplayLogic = new DisplayLogicRequest
            {
                ParentQuestionId = Guid.Empty
            }
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("DisplayLogic.ParentQuestionId");
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.ValidDisplayLogicTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldPass_When_ValidDisplayLogic(DisplayLogicRequest? displayLogic)
    {
        // Arrange
        var multiSelectQuestionRequest = new MultiSelectQuestionRequest
        {
            DisplayLogic = displayLogic
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .Errors
            .Should()
            .AllSatisfy(x =>
            {
                x.PropertyName.Should().NotContain(nameof(MultiSelectQuestionRequest.DisplayLogic));
            });
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
            ],
            DisplayLogic = new DisplayLogicRequest
            {
                ParentQuestionId = Guid.NewGuid(),
                Condition = DisplayLogicCondition.GreaterEqual,
                Value = "1"
            }
        };

        // Act
        var validationResult = _sut.TestValidate(multiSelectQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
