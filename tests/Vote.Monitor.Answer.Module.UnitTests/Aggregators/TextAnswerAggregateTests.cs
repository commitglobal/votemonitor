using FluentAssertions;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators;

public class TextAnswerAggregateTests
{
    private readonly TextQuestion _question = new TextQuestionFaker().Generate();
    private readonly TextAnswerAggregate _aggregate;
    private readonly Guid _responderId = Guid.NewGuid();

    public TextAnswerAggregateTests()
    {
        _aggregate = new TextAnswerAggregate(_question);
    }

    [Fact]
    public void Aggregate_ShouldAddTextAnswer()
    {
        // Arrange
        var answer = TextAnswer.Create(_question.Id, "Test answer");

        // Act
        _aggregate.Aggregate(_responderId, answer);

        // Assert
        _aggregate.Answers.Should().ContainSingle()
            .Which.Responder.Should().Be(_responderId);
        _aggregate.Answers.Should().ContainSingle()
            .Which.Value.Should().Be("Test answer");
    }

    [Fact]
    public void Aggregate_ShouldThrowException_WhenInvalidAnswerReceived()
    {
        // Arrange
        var answer = new TestAnswer(); // Not a TextAnswer

        // Act & Assert
        _aggregate.Invoking(a => a.Aggregate(_responderId, answer))
            .Should().Throw<ArgumentException>()
            .WithMessage($"Invalid answer received: {answer.Discriminator} (Parameter 'answer')");
    }
}
