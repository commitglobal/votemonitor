using FluentAssertions;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators;

public class DateAnswerAggregateTests
{
    private readonly DateQuestion _question = new DateQuestionFaker().Generate();
    private readonly DateAnswerAggregate _aggregate;

    public DateAnswerAggregateTests()
    {
        _aggregate = new DateAnswerAggregate(_question, 0);
    }


    [Fact]
    public void Aggregate_ShouldUpdateHistogram()
    {
        // Arrange
        var submission = new FormSubmissionFaker().Generate();

        var answer1 = DateAnswer.Create(_question.Id, new DateTime(2024, 01, 02, 01, 01, 34, DateTimeKind.Utc)); // bucket 1
        var answer2 = DateAnswer.Create(_question.Id, new DateTime(2024, 01, 02, 01, 02, 50, DateTimeKind.Utc)); // bucket 1

        var answer3 = DateAnswer.Create(_question.Id, new DateTime(2024, 01, 02, 02, 01, 34, DateTimeKind.Utc)); // bucket 2

        var answer4 = DateAnswer.Create(_question.Id, new DateTime(2024, 01, 02, 03, 01, 34, DateTimeKind.Utc)); // bucket 3
        var answer5 = DateAnswer.Create(_question.Id, new DateTime(2024, 01, 02, 03, 01, 34, DateTimeKind.Utc)); // bucket 3

        // Act
        _aggregate.Aggregate(submission, answer1);
        _aggregate.Aggregate(submission, answer2);
        _aggregate.Aggregate(submission, answer3);
        _aggregate.Aggregate(submission, answer4);
        _aggregate.Aggregate(submission, answer5);

        // Assert
        _aggregate.AnswersHistogram["2024-01-02T01:00:00.0000000Z"].Should().Be(2);
        _aggregate.AnswersHistogram["2024-01-02T02:00:00.0000000Z"].Should().Be(1);
        _aggregate.AnswersHistogram["2024-01-02T03:00:00.0000000Z"].Should().Be(2);
    }

    [Fact]
    public void Aggregate_ShouldThrowException_WhenInvalidAnswerReceived()
    {
        // Arrange
        var answer = new TestAnswer(); // Not a DateAnswer

        // Act & Assert
        _aggregate.Invoking(a => a.Aggregate(new FormSubmissionFaker().Generate(), answer))
            .Should().Throw<ArgumentException>()
            .WithMessage($"Invalid answer received: {answer.Discriminator} (Parameter 'answer')");
    }
}
