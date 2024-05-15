using FluentAssertions;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators;

public class DateAnswerAggregateTests
{
    private readonly Guid _responderId = Guid.NewGuid();
    private readonly DateQuestionFaker _question = new DateQuestionFaker();
    private readonly DateAnswerAggregate _aggregate;

    public DateAnswerAggregateTests()
    {
        _aggregate = new DateAnswerAggregate(_question, 0);
    }

    [Fact]
    public void Aggregate_ShouldAddDateAnswerToAnswers()
    {
        // Arrange
        var date = DateTime.Now;
        var answer = new DateAnswerFaker(value: date);

        // Act
        _aggregate.Aggregate(_responderId, answer);

        // Assert
        _aggregate.Answers.Should().ContainSingle()
            .Which.Responder.Should().Be(_responderId);

        _aggregate.Answers.Should().ContainSingle()
            .Which.Value.Should().Be(date);
    }

    [Fact]
    public void Aggregate_ShouldThrowException_WhenInvalidAnswerReceived()
    {
        // Arrange
        var answer = new TestAnswer(); // Not a DateAnswer

        // Act & Assert
        _aggregate.Invoking(a => a.Aggregate(_responderId, answer))
            .Should().Throw<ArgumentException>()
            .WithMessage($"Invalid answer received: {answer.Discriminator} (Parameter 'answer')");
    }
}
