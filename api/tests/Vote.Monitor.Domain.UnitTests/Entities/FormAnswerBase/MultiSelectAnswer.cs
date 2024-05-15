using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAnswerBase;

public class MultiSelectAnswerValidateTests
{
    private readonly Guid _optionId = Guid.NewGuid();
    private readonly MultiSelectQuestion _multiSelectQuestion;

    private readonly NumberQuestion _numberQuestion = NumberQuestion.Create(Guid.NewGuid(), "A", new(), new(), new());

    public MultiSelectAnswerValidateTests()
    {
        IReadOnlyList<SelectOption> options = [new SelectOption(_optionId, new(), false, false)];
        _multiSelectQuestion = MultiSelectQuestion.Create(Guid.NewGuid(), "A", new(), options, new());
    }

    [Fact]
    public void Validate_WithValidQuestion_ReturnsValidationResultWithoutErrors()
    {
        // Arrange
        var answer = new MultiSelectAnswer(Guid.NewGuid(), []);

        // Act
        var validationResult = answer.Validate(_multiSelectQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMatchingQuestionType_ReturnsValidationResultFromValidator()
    {
        // Arrange
        var answer = new MultiSelectAnswer(Guid.NewGuid(), []);

        // Act
        var validationResult = answer.Validate(_multiSelectQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithMismatchedQuestionType_ReturnsInvalidAnswerTypeError()
    {
        // Arrange
        var answer = new MultiSelectAnswer(Guid.NewGuid(), [new SelectedOption(_optionId, string.Empty)]);

        // Act
        var validationResult = answer.Validate(_numberQuestion, 0);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors[0].PropertyName.Should().Be("answers[0].QuestionId");
        validationResult.Errors[0].ErrorMessage.Should().Be("Invalid answer type 'multiSelectAnswer' provided for question of type 'numberQuestion'");
    }

}
