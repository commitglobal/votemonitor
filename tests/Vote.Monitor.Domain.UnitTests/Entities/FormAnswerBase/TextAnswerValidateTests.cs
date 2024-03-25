using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAnswerBase;

public class TextAnswerValidateTests
{
    private readonly TextQuestion _textQuestion = new(Guid.NewGuid(), "A", new(), new(), new());

    readonly NumberQuestion _numberQuestion = new(Guid.NewGuid(), "A", new(), new(), new());

    [Fact]
    public void Validate_WithValidQuestion_ReturnsValidationResultWithoutErrors()
    {
        // Arrange
        var answer = new TextAnswer(Guid.NewGuid(), "an answer");

        // Act
        var validationResult = answer.Validate(_textQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMatchingQuestionType_ReturnsValidationResultFromValidator()
    {
        // Arrange
        var answer = new TextAnswer(Guid.NewGuid(), "an answer");

        // Act
        var validationResult = answer.Validate(_textQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMismatchedQuestionType_ReturnsInvalidAnswerTypeError()
    {
        // Arrange
        var answer = new TextAnswer(Guid.NewGuid(), "an answer");

        // Act
        var validationResult = answer.Validate(_numberQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors[0].PropertyName.Should().Be("answers[0].QuestionId");
        validationResult.Errors[0].ErrorMessage.Should().Be("Invalid answer type 'textAnswer' provided for question of type 'numberQuestion'");
    }
}
