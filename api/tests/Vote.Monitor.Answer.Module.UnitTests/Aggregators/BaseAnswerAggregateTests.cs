using FluentAssertions;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators;


public class BaseAnswerAggregateTests
{
    [Fact]
    public void Aggregate_ShouldIncrementAnswersAggregated()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate());

        // Act
        aggregate.Aggregate(Guid.NewGuid(), new TestAnswer());
        aggregate.Aggregate(Guid.NewGuid(), new TestAnswer());
        aggregate.Aggregate(Guid.NewGuid(), new TestAnswer());

        // Assert
        aggregate.AnswersAggregated.Should().Be(3);
    }

    [Fact]
    public void Aggregate_ShouldAddResponderIdToResponders()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate());
        var responderId1 = Guid.NewGuid();
        var responderId2 = Guid.NewGuid();

        // Act
        aggregate.Aggregate(responderId1, new TestAnswer());
        aggregate.Aggregate(responderId2, new TestAnswer());

        // Assert
        aggregate.Responders.Should().HaveCount(2);
        aggregate.Responders.Should().Contain(responderId1);
        aggregate.Responders.Should().Contain(responderId2);
    }

    [Fact]
    public void Aggregate_ShouldInvokeQuestionSpecificAggregate()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate());
        var responderId = Guid.NewGuid();

        // Act
        aggregate.Aggregate(responderId, new TestAnswer());

        // Assert
        aggregate.QuestionSpecificAggregateInvoked.Should().BeTrue();
    }
}

public class TestAnswerAggregate : BaseAnswerAggregate
{
    public bool QuestionSpecificAggregateInvoked { get; private set; }

    public TestAnswerAggregate(BaseQuestion question) : base(question)
    {
    }

    protected override void QuestionSpecificAggregate(Guid responder, BaseAnswer answer)
    {
        QuestionSpecificAggregateInvoked = true;
    }
}
