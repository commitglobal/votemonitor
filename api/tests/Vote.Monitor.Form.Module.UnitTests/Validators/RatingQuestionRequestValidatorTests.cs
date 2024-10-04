﻿using FluentAssertions;
using FluentValidation.TestHelper;
using Vote.Monitor.Core.Constants;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
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
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases),
        MemberType = typeof(ValidatorsTestData))]
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
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases),
        MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_InvalidHelptext(TranslatedString invalidHelptext)
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

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases),
        MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_InvalidUpperLabel(TranslatedString invalidUpperLabel)
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            UpperLabel = invalidUpperLabel
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.UpperLabel);
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.InvalidPartiallyTranslatedTestCases),
        MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldFail_When_InvalidLowerLabel(TranslatedString invalidLowerLabel)
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            LowerLabel = invalidLowerLabel
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor(x => x.LowerLabel);
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
    [MemberData(nameof(ValidatorsTestData.InvalidCodeTestCases), MemberType = typeof(ValidatorsTestData))]
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
    public void Validation_ShouldFail_When_InvalidDisplayLogic()
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            DisplayLogic = new DisplayLogicRequest
            {
                ParentQuestionId = Guid.Empty
            }
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldHaveValidationErrorFor("DisplayLogic.ParentQuestionId");
    }

    [Theory]
    [MemberData(nameof(ValidatorsTestData.ValidDisplayLogicTestCases), MemberType = typeof(ValidatorsTestData))]
    public void Validation_ShouldPass_When_ValidDisplayLogic(DisplayLogicRequest? displayLogic)
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            DisplayLogic = displayLogic
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .Errors
            .Should()
            .AllSatisfy(x => { x.PropertyName.Should().NotContain(nameof(MultiSelectQuestionRequest.DisplayLogic)); });
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var ratingQuestionRequest = new RatingQuestionRequest
        {
            Scale = RatingScale.OneTo10,
            Helptext = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Text = ValidatorsTestData.ValidPartiallyTranslatedTestData.First(),
            Code = "A code",
            Id = Guid.NewGuid(),
            DisplayLogic = new DisplayLogicRequest
            {
                ParentQuestionId = Guid.NewGuid(),
                Condition = DisplayLogicCondition.GreaterEqual,
                Value = "1"
            }
        };

        // Act
        var validationResult = _sut.TestValidate(ratingQuestionRequest);

        // Assert
        validationResult
            .ShouldNotHaveValidationErrorFor(x => x.Scale);
    }
}