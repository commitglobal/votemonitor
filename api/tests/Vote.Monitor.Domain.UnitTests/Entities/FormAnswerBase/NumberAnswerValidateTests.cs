using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAnswerBase;

public class NumberAnswerValidateTests
{
    readonly NumberQuestion _numberQuestion = NumberQuestion.Create(Guid.NewGuid(), "A", new(), new(), new());
    private readonly RatingQuestion _ratingQuestion = RatingQuestion.Create(Guid.NewGuid(), "A", new(), RatingScale.OneTo3, new());

    [Fact]
    public void Validate_WithValidQuestion_ReturnsValidationResultWithoutErrors()
    {
        // Arrange
        var answer = new NumberAnswer(Guid.NewGuid(), 2);

        // Act
        var validationResult = answer.Validate(_numberQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMatchingQuestionType_ReturnsValidationResultFromValidator()
    {
        // Arrange
        var answer = new NumberAnswer(Guid.NewGuid(), 2);

        // Act
        var validationResult = answer.Validate(_numberQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMismatchedQuestionType_ReturnsInvalidAnswerTypeError()
    {
        // Arrange
        var answer = new NumberAnswer(Guid.NewGuid(), 2);

        // Act
        var validationResult = answer.Validate(_ratingQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors[0].PropertyName.Should().Be("answers[0].QuestionId");
        validationResult.Errors[0].ErrorMessage.Should().Be("Invalid answer type 'numberAnswer' provided for question of type 'ratingQuestion'");
    }
}
