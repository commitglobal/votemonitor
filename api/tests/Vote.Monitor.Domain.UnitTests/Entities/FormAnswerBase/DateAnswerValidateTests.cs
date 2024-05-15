using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAnswerBase;

public class DateAnswerValidateTests
{
    private readonly DateQuestion _dateQuestion = DateQuestion.Create(Guid.NewGuid(), "A", new(), new());

    readonly NumberQuestion _numberQuestion = NumberQuestion.Create(Guid.NewGuid(), "A", new(), new(), new());

    [Fact]
    public void Validate_WithValidQuestion_ReturnsValidationResultWithoutErrors()
    {
        // Arrange
        var answer = new DateAnswer(Guid.NewGuid(), DateTime.UtcNow);

        // Act
        var validationResult = answer.Validate(_dateQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMatchingQuestionType_ReturnsValidationResultFromValidator()
    {
        // Arrange
        var answer = new DateAnswer(Guid.NewGuid(), DateTime.UtcNow);

        // Act
        var validationResult = answer.Validate(_dateQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMismatchedQuestionType_ReturnsInvalidAnswerTypeError()
    {
        // Arrange
        var answer = new DateAnswer(Guid.NewGuid(), DateTime.UtcNow);

        // Act
        var validationResult = answer.Validate(_numberQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors[0].PropertyName.Should().Be("answers[0].QuestionId");
        validationResult.Errors[0].ErrorMessage.Should().Be("Invalid answer type 'dateAnswer' provided for question of type 'numberQuestion'");
    }
}
