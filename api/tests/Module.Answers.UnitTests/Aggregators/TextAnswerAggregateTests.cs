using FluentAssertions;
using Module.Answers.Aggregators;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Xunit;

namespace Module.Answers.UnitTests.Aggregators;

public class TextAnswerAggregateTests
{
    private readonly TextQuestion _question = new TextQuestionFaker().Generate();
    private readonly TextAnswerAggregate _aggregate;
    private readonly FormSubmission _submission = new FormSubmissionFaker().Generate();

    public TextAnswerAggregateTests()
    {
        _aggregate = new TextAnswerAggregate(_question, 0);
    }

    [Fact]
    public void Aggregate_ShouldAddTextAnswer()
    {
        // Arrange
        var answer = TextAnswer.Create(_question.Id, "Test answer");

        // Act
        _aggregate.Aggregate(_submission.Id, _submission.MonitoringObserverId, answer);

        // Assert
        _aggregate.Answers.Should().ContainSingle()
            .Which.SubmissionId.Should().Be(_submission.Id);
        _aggregate.Answers.Should().ContainSingle()
            .Which.ResponderId.Should().Be(_submission.MonitoringObserverId);
        _aggregate.Answers.Should().ContainSingle()
            .Which.Value.Should().Be("Test answer");
    }

    [Fact]
    public void Aggregate_ShouldThrowException_WhenInvalidAnswerReceived()
    {
        // Arrange
        var answer = new TestAnswer(); // Not a TextAnswer

        // Act & Assert
        _aggregate.Invoking(a => a.Aggregate(Guid.NewGuid(), Guid.NewGuid(), answer))
            .Should().Throw<ArgumentException>()
            .WithMessage($"Invalid answer received: {answer.Discriminator} (Parameter 'answer')");
    }
}
