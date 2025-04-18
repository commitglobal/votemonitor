using FluentAssertions;
using FluentValidation.TestHelper;
using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Module.Forms.Requests;
using Module.Forms.Validators;

namespace Module.Forms.UnitTests.Validators;

public class DateQuestionRequestValidatorTests
{
    private readonly DateQuestionRequestValidator _sut = new([LanguagesList.EN.Iso1, LanguagesList.RO.Iso1]);

    [Fact]
    public void Validation_ShouldFail_When_EmptyId()
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Id = Guid.Empty
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyText(TranslatedString invalidText)
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Text = invalidText
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_EmptyHelptext(TranslatedString invalidHelptext)
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Helptext = invalidHelptext
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Helptext);
    }

    [Fact]
    public void Validation_ShouldPass_When_NoHelptext()
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Helptext = null
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Helptext);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_CodeEmpty(string code)
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

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
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Code = code
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.Code);
    }


    [Fact]
    public void Validation_ShouldFail_When_InvalidDisplayLogic()
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            DisplayLogic = new DisplayLogicRequest
            {
                ParentQuestionId = Guid.Empty
            }
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("DisplayLogic.ParentQuestionId");
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.ValidDisplayLogicTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldPass_When_ValidDisplayLogic(DisplayLogicRequest? displayLogic)
    {
        // Arrange
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            DisplayLogic = displayLogic
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

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
        var dateInputQuestionRequest = new DateQuestionRequest
        {
            Code = "code",
            Helptext = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.Last(),
            Id = Guid.NewGuid(),
            DisplayLogic = new DisplayLogicRequest
            {
                ParentQuestionId = Guid.NewGuid(),
                Condition = DisplayLogicCondition.GreaterEqual,
                Value = "1"
            }
        };

        // Act
        var validationResult = _sut.TestValidate(dateInputQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveAnyValidationErrors();
    }
}
