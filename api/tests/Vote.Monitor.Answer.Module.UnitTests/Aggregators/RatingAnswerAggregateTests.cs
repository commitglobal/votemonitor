using FluentAssertions;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators;

public class RatingAnswerAggregateTests
{
    private readonly RatingQuestion _question = new RatingQuestionFaker(RatingScale.OneTo10).Generate();
    private readonly RatingAnswerAggregate _aggregate;
    private readonly FormSubmission _submission = new FormSubmissionFaker().Generate();

    public RatingAnswerAggregateTests()
    {
        _aggregate = new RatingAnswerAggregate(_question, 0);
    }

    [Theory]
    [MemberData(nameof(RatingAnswerTesCases))]
    public void Aggregate_ShouldInitializeHistogram(RatingScale scale, int count)
    {
        // Arrange
        var question = new RatingQuestionFaker(scale).Generate();

        // Act
        var aggregate = new RatingAnswerAggregate(question, 0);

        // Assert
        aggregate.AnswersHistogram.Should().HaveCount(count);
        aggregate.AnswersHistogram.Keys.Should().BeEquivalentTo(Enumerable.Range(1, count));
        aggregate.AnswersHistogram.Values.Should().AllSatisfy(value => value.Should().Be(0));
    }

    public static IEnumerable<object[]> RatingAnswerTesCases =>
        new List<object[]>
        {
            new object[] { RatingScale.OneTo3, 3 },
            new object[] { RatingScale.OneTo4, 4 },
            new object[] { RatingScale.OneTo5, 5 },
            new object[] { RatingScale.OneTo6, 6 },
            new object[] { RatingScale.OneTo7, 7 },
            new object[] { RatingScale.OneTo8, 8 },
            new object[] { RatingScale.OneTo9, 9 },
            new object[] { RatingScale.OneTo10, 10 }
        };

    [Fact]
    public void Aggregate_ShouldUpdateHistogram()
    {
        // Arrange
        var answer1 = RatingAnswer.Create(_question.Id, 10);
        var answer2 = RatingAnswer.Create(_question.Id, 10);
        var answer3 = RatingAnswer.Create(_question.Id, 2);
        var answer4 = RatingAnswer.Create(_question.Id, 3);

        // Act
        _aggregate.Aggregate(_submission, answer1);
        _aggregate.Aggregate(_submission, answer2);
        _aggregate.Aggregate(_submission, answer3);
        _aggregate.Aggregate(_submission, answer4);

        // Assert
        _aggregate.AnswersHistogram[1].Should().Be(0);
        _aggregate.AnswersHistogram[2].Should().Be(1);
        _aggregate.AnswersHistogram[3].Should().Be(1);
        _aggregate.AnswersHistogram[4].Should().Be(0);
        _aggregate.AnswersHistogram[5].Should().Be(0);
        _aggregate.AnswersHistogram[6].Should().Be(0);
        _aggregate.AnswersHistogram[7].Should().Be(0);
        _aggregate.AnswersHistogram[8].Should().Be(0);
        _aggregate.AnswersHistogram[9].Should().Be(0);
        _aggregate.AnswersHistogram[10].Should().Be(2);
    }

    [Fact]
    public void Aggregate_ShouldUpdateMin()
    {
        // Arrange
        var answer1 = RatingAnswer.Create(_question.Id, 10);
        var answer2 = RatingAnswer.Create(_question.Id, 5);

        // Act
        _aggregate.Aggregate(_submission, answer1);
        _aggregate.Aggregate(_submission, answer2);

        // Assert
        _aggregate.Min.Should().Be(5);
    }

    [Fact]
    public void Aggregate_ShouldUpdateMax()
    {
        // Arrange
        var answer1 = RatingAnswer.Create(_question.Id, 10);
        var answer2 = RatingAnswer.Create(_question.Id, 2);

        // Act
        _aggregate.Aggregate(_submission, answer1);
        _aggregate.Aggregate(_submission, answer2);

        // Assert
        _aggregate.Max.Should().Be(10);
    }

    [Fact]
    public void Aggregate_ShouldUpdateAverage()
    {
        // Arrange
        var answer1 = RatingAnswer.Create(_question.Id, 10);
        var answer2 = RatingAnswer.Create(_question.Id, 2);

        // Act
        _aggregate.Aggregate(_submission, answer1);
        _aggregate.Aggregate(_submission, answer2);

        // Assert
        _aggregate.Average.Should().Be(6);
    }

    [Fact]
    public void Aggregate_ShouldThrowException_WhenInvalidAnswerReceived()
    {
        // Arrange
        var answer = new TestAnswer(); // Not a RatingAnswer

        // Act & Assert
        _aggregate.Invoking(a => a.Aggregate(_submission, answer))
            .Should().Throw<ArgumentException>()
            .WithMessage($"Invalid answer received: {answer.Discriminator} (Parameter 'answer')");
    }
}
