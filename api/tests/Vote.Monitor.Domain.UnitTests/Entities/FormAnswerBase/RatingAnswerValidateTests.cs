using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAnswerBase;

public class RatingAnswerValidateTests
{
    private readonly RatingQuestion _ratingQuestion = RatingQuestion.Create(Guid.NewGuid(), "A", new(), RatingScale.OneTo3);

    readonly NumberQuestion _numberQuestion = NumberQuestion.Create(Guid.NewGuid(), "A", new());

    [Fact]
    public void Validate_WithValidQuestion_ReturnsValidationResultWithoutErrors()
    {
        // Arrange
        var answer = new RatingAnswer(Guid.NewGuid(), 2);

        // Act
        var validationResult = answer.Validate(_ratingQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMatchingQuestionType_ReturnsValidationResultFromValidator()
    {
        // Arrange
        var answer = new RatingAnswer(Guid.NewGuid(), 2);

        // Act
        var validationResult = answer.Validate(_ratingQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMismatchedQuestionType_ReturnsInvalidAnswerTypeError()
    {
        // Arrange
        var answer = new RatingAnswer(Guid.NewGuid(), 2);

        // Act
        var validationResult = answer.Validate(_numberQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors[0].PropertyName.Should().Be("answers[0].QuestionId");
        validationResult.Errors[0].ErrorMessage.Should().Be("Invalid answer type 'ratingAnswer' provided for question of type 'numberQuestion'");
    }
}
