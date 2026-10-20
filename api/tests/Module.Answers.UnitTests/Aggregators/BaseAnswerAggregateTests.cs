using System.Text.Json;
using FluentAssertions;
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

    [Fact]
    public void Aggregate_ShouldCountDistinctPollingStations()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate(), 0);
        var pollingStation1 = Guid.NewGuid();
        var pollingStation2 = Guid.NewGuid();

        // Act
        aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), new TestAnswer(), pollingStation1);
        aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), new TestAnswer(), pollingStation1);
        aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), new TestAnswer(), pollingStation2);

        // Assert
        aggregate.AnswersAggregated.Should().Be(3);
        aggregate.PollingStationsCount.Should().Be(2);
        aggregate.LocationsCount.Should().BeNull();
    }

    [Fact]
    public void Aggregate_ShouldCountDistinctLocations()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate(), 0);
        var location1 = Guid.NewGuid();
        var location2 = Guid.NewGuid();

        // Act
        aggregate.Aggregate(Guid.NewGuid(), Guid.Empty, new TestAnswer(), locationId: location1);
        aggregate.Aggregate(Guid.NewGuid(), Guid.Empty, new TestAnswer(), locationId: location2);
        aggregate.Aggregate(Guid.NewGuid(), Guid.Empty, new TestAnswer(), locationId: location2);

        // Assert
        aggregate.LocationsCount.Should().Be(2);
        aggregate.PollingStationsCount.Should().BeNull();
    }

    [Fact]
    public void Aggregate_ShouldNotSetPlaceCounts_WhenThereAreNoPollingStationsOrLocations()
    {
        // Arrange
        var aggregate = new TestAnswerAggregate(new TextQuestionFaker().Generate(), 0);

        // Act
        aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), new TestAnswer());

        // Assert
        aggregate.PollingStationsCount.Should().BeNull();
        aggregate.LocationsCount.Should().BeNull();
    }

    [Fact]
    public void Deserialize_ShouldKeepPlaceCountsComputedByTheDatabase()
    {
        // Arrange
        var json = $$"""
                     {
                       "$questionType": "numberAnswerAggregate",
                       "QuestionId": "{{Guid.NewGuid()}}",
                       "AnswersAggregated": 3,
                       "PollingStationsCount": 2,
                       "LocationsCount": 1,
                       "LocationAggregates": {
                         "1": { "$questionType": "numberAnswerAggregate", "AnswersAggregated": 1, "PollingStationsCount": 1 }
                       }
                     }
                     """;

        // Act
        var aggregate = JsonSerializer.Deserialize<BaseAnswerAggregate>(json)!;

        // Assert
        aggregate.PollingStationsCount.Should().Be(2);
        aggregate.LocationsCount.Should().Be(1);
        aggregate.LocationAggregates![1].PollingStationsCount.Should().Be(1);
        aggregate.LocationAggregates[1].LocationsCount.Should().BeNull();
    }
}

public class TestAnswerAggregate : BaseAnswerAggregate
{
    public bool QuestionSpecificAggregateInvoked { get; private set; }

    public TestAnswerAggregate(BaseQuestion question, int displayOrder) : base(question, displayOrder)
    {
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer, Guid? pollingStationId)
    {
        QuestionSpecificAggregateInvoked = true;
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer, Guid? pollingStationId)
    {
        throw new NotImplementedException();
    }
}
