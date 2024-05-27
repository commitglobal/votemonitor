using FluentAssertions;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators;

public class BaseAnswerAggregateTests
{
    [Fact]
    public void Aggregate_ShouldIncrementAnswersAggregated()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate(), 0);

        // Act
        aggregate.Aggregate(new FormSubmissionFaker().Generate(), new TestAnswer());
        aggregate.Aggregate(new FormSubmissionFaker().Generate(), new TestAnswer());
        aggregate.Aggregate(new FormSubmissionFaker().Generate(), new TestAnswer());

        // Assert
        aggregate.AnswersAggregated.Should().Be(3);
    }

    [Fact]
    public void Aggregate_ShouldAddResponderIdToResponders()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate(), 0);
        var submission1 = new FormSubmissionFaker().Generate();
        var submission2 = new FormSubmissionFaker().Generate();

        // Act
        aggregate.Aggregate(submission1, new TestAnswer());
        aggregate.Aggregate(submission2, new TestAnswer());

        // Assert
        aggregate.Responders.Should().HaveCount(2);
        aggregate.Responders.Should().Contain(submission1.MonitoringObserverId);
        aggregate.Responders.Should().Contain(submission2.MonitoringObserverId);
    }

    [Fact]
    public void Aggregate_ShouldInvokeQuestionSpecificAggregate()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate(), 0);

        // Act
        aggregate.Aggregate(new FormSubmissionFaker().Generate(), new TestAnswer());

        // Assert
        aggregate.QuestionSpecificAggregateInvoked.Should().BeTrue();
    }
}

public class TestAnswerAggregate : BaseAnswerAggregate
{
    public bool QuestionSpecificAggregateInvoked { get; private set; }

    public TestAnswerAggregate(BaseQuestion question, int displayOrder) : base(question, displayOrder)
    {
    }

    protected override void QuestionSpecificAggregate(FormSubmission submission, BaseAnswer answer)
    {
        QuestionSpecificAggregateInvoked = true;
    }
}
