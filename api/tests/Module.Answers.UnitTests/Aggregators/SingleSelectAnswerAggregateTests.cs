using FluentAssertions;
using Module.Answers.Aggregators;
using Module.Answers.UnitTests.Aggregators.Extensions;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Xunit;

namespace Module.Answers.UnitTests.Aggregators;

public class SingleSelectAnswerAggregateTests
{
    private readonly int _optionsCount = 10;
    private readonly List<SelectOption> _options;
    private readonly SelectOption _freeTextOption;
    private readonly SingleSelectQuestion _question;
    private readonly SingleSelectAnswerAggregate _aggregate;
    private readonly FormSubmission _submission = new FormSubmissionFaker().Generate();


    public SingleSelectAnswerAggregateTests()
    {
        _options = new SelectOptionFaker().Generate(_optionsCount);
        _freeTextOption = SelectOption.Create(Guid.NewGuid(), new TranslatedString(), isFreeText: true);
        _options.Add(_freeTextOption);
        _question = new SingleSelectQuestionFaker(options: _options).Generate();
        _aggregate = new SingleSelectAnswerAggregate(_question, 0);
    }

    [Fact]
    public void Aggregate_ShouldInitializeHistogram()
    {
        // Assert
        _aggregate.AnswersHistogram.Should().HaveCount(_optionsCount + 1);
        _aggregate.AnswersHistogram.Keys.Should().BeEquivalentTo(_options.Select(x => x.Id));
        _aggregate.AnswersHistogram.Values.Should().AllSatisfy(value => value.Should().Be(0));
        _aggregate.FreeTexts.Should().BeEmpty();
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
        _aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), answer1);
        _aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), answer2);
        _aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), answer3);
        _aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), answer4);

        // Assert
        _aggregate.AnswersHistogram[option1.Id].Should().Be(2);
        _aggregate.AnswersHistogram[option3.Id].Should().Be(1);
        _aggregate.AnswersHistogram[option5.Id].Should().Be(1);
        _aggregate.AnswersHistogram.Values.Where(x => x == 0).Should().HaveCount(8);
    }

    [Fact]
    public void Aggregate_ShouldCollectNonEmptyFreeTexts_WhenOptionIsFreeText()
    {
        // Arrange
        var answerWithText = SingleSelectAnswer.Create(_question.Id, SelectedOption.Create(_freeTextOption.Id, "some free text"));
        var answerWithEmptyText = SingleSelectAnswer.Create(_question.Id, SelectedOption.Create(_freeTextOption.Id, ""));
        var answerWithWhitespace = SingleSelectAnswer.Create(_question.Id, SelectedOption.Create(_freeTextOption.Id, "   "));
        var answerWithoutText = SingleSelectAnswer.Create(_question.Id, _freeTextOption.Select());

        // Act
        _aggregate.Aggregate(_submission.Id, _submission.MonitoringObserverId, answerWithText);
        _aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), answerWithEmptyText);
        _aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), answerWithWhitespace);
        _aggregate.Aggregate(Guid.NewGuid(), Guid.NewGuid(), answerWithoutText);

        // Assert
        _aggregate.FreeTexts.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new TextResponse(_submission.Id, _submission.MonitoringObserverId, "some free text"));
    }

    [Fact]
    public void Aggregate_ShouldNotCollectFreeTexts_WhenOptionIsNotFreeText()
    {
        // Arrange
        var option = _options[1];
        var answer = SingleSelectAnswer.Create(_question.Id, SelectedOption.Create(option.Id, "should be ignored"));

        // Act
        _aggregate.Aggregate(_submission.Id, _submission.MonitoringObserverId, answer);

        // Assert
        _aggregate.FreeTexts.Should().BeEmpty();
    }

    [Fact]
    public void Aggregate_ShouldThrowException_WhenInvalidAnswerReceived()
    {
        // Arrange
        var answer = new TestAnswer(); // Not a SingleSelectAnswer

        // Act & Assert
        _aggregate.Invoking(a => a.Aggregate(Guid.NewGuid(), Guid.NewGuid(), answer))
            .Should().Throw<ArgumentException>()
            .WithMessage($"Invalid answer received: {answer.Discriminator} (Parameter 'answer')");
    }
}
