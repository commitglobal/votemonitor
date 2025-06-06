﻿using FluentAssertions;
using Module.Answers.Aggregators;
using Module.Answers.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Xunit;

namespace Module.Answers.UnitTests.Aggregators;

public class BaseAnswerAggregateTests
{
    [Fact]
    public void Aggregate_ShouldIncrementAnswersAggregated()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate(), 0);

        // Act
        aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), new TestAnswer());
        aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), new TestAnswer());
        aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), new TestAnswer());

        // Assert
        aggregate.AnswersAggregated.Should().Be(3);
    }

    [Fact]
    public void Aggregate_ShouldInvokeQuestionSpecificAggregate()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate(), 0);

        // Act
        aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), new TestAnswer());

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

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        QuestionSpecificAggregateInvoked = true;
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer)
    {
        throw new NotImplementedException();
    }
}
