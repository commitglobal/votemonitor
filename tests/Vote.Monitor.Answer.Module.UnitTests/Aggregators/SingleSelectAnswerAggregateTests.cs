﻿using FluentAssertions;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Answer.Module.UnitTests.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators;

public class SingleSelectAnswerAggregateTests
{
    private readonly int _optionsCount = 10;
    private readonly List<SelectOption> _options;
    private readonly SingleSelectQuestion _question;
    private readonly SingleSelectAnswerAggregate _aggregate;
    private readonly Guid _responderId = Guid.NewGuid();

    public SingleSelectAnswerAggregateTests()
    {
        _options = new SelectOptionFaker().Generate(_optionsCount);
        _question = new SingleSelectQuestionFaker(options: _options).Generate();
        _aggregate = new SingleSelectAnswerAggregate(_question);
    }

    [Fact]
    public void Aggregate_ShouldInitializeHistogram()
    {
        // Assert
        _aggregate.AnswersHistogram.Should().HaveCount(_optionsCount);
        _aggregate.AnswersHistogram.Keys.Should().BeEquivalentTo(_options.Select(x => x.Id));
        _aggregate.AnswersHistogram.Values.Should().AllSatisfy(value => value.Should().Be(0));
    }

    [Fact]
    public void Aggregate_ShouldAddSingleSelectAnswer()
    {
        // Arrange
        var selection = _options[4].Select();
        var answer = SingleSelectAnswer.Create(_question.Id, selection);

        // Act
        _aggregate.Aggregate(_responderId, answer);

        // Assert
        _aggregate.Answers.Should().ContainSingle()
            .Which.Responder.Should().Be(_responderId);
        _aggregate.Answers.Should().ContainSingle()
            .Which.Value.Should().Be(selection);
    }

    [Fact]
    public void Aggregate_ShouldUpdateHistogram()
    {
        // Arrange
        var option1 = _options[1];
        var option3 = _options[3];
        var option5 = _options[5];

        var answer1 = SingleSelectAnswer.Create(_question.Id, option1.Select());
        var answer2 = SingleSelectAnswer.Create(_question.Id, option1.Select());
        var answer3 = SingleSelectAnswer.Create(_question.Id, option3.Select());
        var answer4 = SingleSelectAnswer.Create(_question.Id, option5.Select());

        // Act
        _aggregate.Aggregate(_responderId, answer1);
        _aggregate.Aggregate(_responderId, answer2);
        _aggregate.Aggregate(_responderId, answer3);
        _aggregate.Aggregate(_responderId, answer4);

        // Assert
        _aggregate.AnswersHistogram[option1.Id].Should().Be(2);
        _aggregate.AnswersHistogram[option3.Id].Should().Be(1);
        _aggregate.AnswersHistogram[option5.Id].Should().Be(1);
        _aggregate.AnswersHistogram.Values.Where(x => x == 0).Should().HaveCount(7);
    }

    [Fact]
    public void Aggregate_ShouldThrowException_WhenInvalidAnswerReceived()
    {
        // Arrange
        var answer = new TestAnswer(); // Not a SingleSelectAnswer

        // Act & Assert
        _aggregate.Invoking(a => a.Aggregate(_responderId, answer))
            .Should().Throw<ArgumentException>()
            .WithMessage($"Invalid answer received: {answer.Discriminator} (Parameter 'answer')");
    }
}
