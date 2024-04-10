using FluentAssertions;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Answer.Module.UnitTests.Aggregators.Extensions;
using Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Xunit;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators;

public class FormAggregateTests
{
    private readonly Form _form;
    private readonly TextQuestion _textQuestion = new TextQuestionFaker().Generate();
    private readonly NumberQuestion _numberQuestion = new NumberQuestionFaker().Generate();
    private readonly DateQuestion _dateQuestion = new DateQuestionFaker().Generate();
    private readonly RatingQuestion _ratingQuestion = new RatingQuestionFaker().Generate();
    private readonly SingleSelectQuestion _singleSelectQuestion;
    private readonly MultiSelectQuestion _multiSelectQuestion;
    private readonly ElectionRound _electionRound = new ElectionRoundAggregateFaker().Generate();

    public FormAggregateTests()
    {
        var monitoringNgo = new MonitoringNgoAggregateFaker(electionRound: _electionRound).Generate();
        var singleSelectOptions = new SelectOptionFaker().Generate(4);
        var multiSelectOptions = new SelectOptionFaker().Generate(4);

        _singleSelectQuestion = new SingleSelectQuestionFaker(singleSelectOptions).Generate();
        _multiSelectQuestion = new MultiSelectQuestionFaker(multiSelectOptions).Generate();
        
        List<BaseQuestion> questions = [
            _textQuestion,
            _numberQuestion,
            _dateQuestion,
            _ratingQuestion,
            _singleSelectQuestion,
            _multiSelectQuestion
        ];

        _form = Form.Create(_electionRound, monitoringNgo, FormType.Opening, "F1", new TranslatedStringFaker(), [], questions);
    }

    [Fact]
    public void Constructor_ShouldCreateEmptyAggregates()
    {
        // Act
        var aggregate = new FormAggregate(_form);

        // Assert
        aggregate.Aggregates.Should().HaveCount(6);
        aggregate.SubmissionCount.Should().Be(0);

        aggregate
            .Aggregates[_textQuestion.Id]
            .Should().BeOfType<TextAnswerAggregate>()
            .Which.QuestionId
            .Should().Be(_textQuestion.Id);

        aggregate
            .Aggregates[_numberQuestion.Id]
            .Should().BeOfType<NumberAnswerAggregate>()
            .Which.QuestionId
            .Should().Be(_numberQuestion.Id);

        aggregate
            .Aggregates[_ratingQuestion.Id]
            .Should().BeOfType<RatingAnswerAggregate>()
            .Which.QuestionId
            .Should().Be(_ratingQuestion.Id);

        aggregate
            .Aggregates[_dateQuestion.Id]
            .Should().BeOfType<DateAnswerAggregate>()
            .Which.QuestionId
            .Should().Be(_dateQuestion.Id);

        aggregate
            .Aggregates[_singleSelectQuestion.Id]
            .Should().BeOfType<SingleSelectAnswerAggregate>()
            .Which.QuestionId
            .Should().Be(_singleSelectQuestion.Id);

        aggregate
            .Aggregates[_multiSelectQuestion.Id]
            .Should().BeOfType<MultiSelectAnswerAggregate>()
            .Which.QuestionId
            .Should().Be(_multiSelectQuestion.Id);

        aggregate.ElectionRoundId.Should().Be(_form.ElectionRoundId);
        aggregate.MonitoringNgoId.Should().Be(_form.MonitoringNgoId);
        aggregate.FormId.Should().Be(_form.Id);
    }

    [Fact]
    public void AggregateAnswers_ShouldIncrementSubmissionCount()
    {
        // Arrange
        var aggregate = new FormAggregate(_form);

        var pollingStation = new PollingStationFaker(electionRound: _electionRound).Generate();
        var monitoringObserver = new MonitoringObserverFaker().Generate();
        var formSubmission1 = FormSubmission.Create(_electionRound, pollingStation, monitoringObserver, _form, []);
        var formSubmission2 = FormSubmission.Create(_electionRound, pollingStation, monitoringObserver, _form, []);

        // Act
        aggregate.AggregateAnswers(formSubmission1);
        aggregate.AggregateAnswers(formSubmission2);

        // Assert
        aggregate.SubmissionCount.Should().Be(2);
    }

    [Fact]
    public void AggregateAnswers_ShouldAddResponderIdToResponders()
    {
        // Arrange
        var aggregate = new FormAggregate(_form);

        var pollingStation = new PollingStationFaker(electionRound: _electionRound).Generate();

        var monitoringObserver1 = new MonitoringObserverFaker().Generate();
        var monitoringObserver2 = new MonitoringObserverFaker().Generate();

        var formSubmission1 = FormSubmission.Create(_electionRound, pollingStation, monitoringObserver1, _form, []);
        var formSubmission2 = FormSubmission.Create(_electionRound, pollingStation, monitoringObserver2, _form, []);
        var formSubmission3 = FormSubmission.Create(_electionRound, pollingStation, monitoringObserver1, _form, []);

        // Act
        aggregate.AggregateAnswers(formSubmission1);
        aggregate.AggregateAnswers(formSubmission2);
        aggregate.AggregateAnswers(formSubmission3);

        // Assert
        aggregate.Responders.Should().HaveCount(2);
        aggregate.Responders.Should().Contain([monitoringObserver1.Id, monitoringObserver2.Id]);
    }

    [Fact]
    public void AggregateAnswers_ShouldAddPollingStationIdToPollingStations()
    {
        // Arrange
        var aggregate = new FormAggregate(_form);

        var pollingStation1 = new PollingStationFaker(electionRound: _electionRound).Generate();
        var pollingStation2 = new PollingStationFaker(electionRound: _electionRound).Generate();

        var monitoringObserver = new MonitoringObserverFaker().Generate();

        var formSubmission1 = FormSubmission.Create(_electionRound, pollingStation1, monitoringObserver, _form, []);
        var formSubmission2 = FormSubmission.Create(_electionRound, pollingStation2, monitoringObserver, _form, []);
        var formSubmission3 = FormSubmission.Create(_electionRound, pollingStation1, monitoringObserver, _form, []);

        // Act
        aggregate.AggregateAnswers(formSubmission1);
        aggregate.AggregateAnswers(formSubmission2);
        aggregate.AggregateAnswers(formSubmission3);

        // Assert
        aggregate.PollingStations.Keys.Should().HaveCount(2);
        aggregate.PollingStations.Keys.Should().Contain([pollingStation1.Id, pollingStation2.Id]);
        aggregate.PollingStations[pollingStation1.Id]
            .Should().HaveCount(2)
            .And.Contain([formSubmission1.Id, formSubmission3.Id]);

        aggregate.PollingStations[pollingStation2.Id]
            .Should().HaveCount(1)
            .And.Contain(formSubmission2.Id);
    }

    [Fact]
    public void AggregateAnswers_ShouldAggregateAnswers()
    {
        // Arrange
        var aggregate = new FormAggregate(_form);
        var pollingStation = new PollingStationFaker(electionRound: _electionRound).Generate();
        var monitoringObserver = new MonitoringObserverFaker().Generate();

        List<BaseAnswer> submission1Answers = [
            new DateAnswerFaker(_dateQuestion.Id),
            new TextAnswerFaker(_textQuestion.Id),
            new RatingAnswerFaker(_ratingQuestion.Id),
            new NumberAnswerFaker(_numberQuestion.Id),
            new SingleSelectAnswerFaker(_singleSelectQuestion.Id, _singleSelectQuestion.Options[1].Select()),
            new MultiSelectAnswerFaker(_multiSelectQuestion.Id, _multiSelectQuestion.Options.Select(o => o.Select()))
        ];

        List<BaseAnswer> submission2Answers = [
            new SingleSelectAnswerFaker(_singleSelectQuestion.Id, _singleSelectQuestion.Options[2].Select()),
            new MultiSelectAnswerFaker(_multiSelectQuestion.Id, _multiSelectQuestion.Options.Select(o=>o.Select()))
        ];

        List<BaseAnswer> submission3Answers = [
            new DateAnswerFaker(_dateQuestion.Id),
            new TextAnswerFaker(_textQuestion.Id),
            new NumberAnswerFaker(_numberQuestion.Id),
        ];

        var formSubmission1 = FormSubmission.Create(_electionRound, pollingStation, monitoringObserver, _form, submission1Answers);
        var formSubmission2 = FormSubmission.Create(_electionRound, pollingStation, monitoringObserver, _form, submission2Answers);
        var formSubmission3 = FormSubmission.Create(_electionRound, pollingStation, monitoringObserver, _form, submission3Answers);

        // Act
        aggregate.AggregateAnswers(formSubmission1);
        aggregate.AggregateAnswers(formSubmission2);
        aggregate.AggregateAnswers(formSubmission3);

        // Assert
        aggregate.Aggregates[_textQuestion.Id].AnswersAggregated.Should().Be(2);
        aggregate.Aggregates[_dateQuestion.Id].AnswersAggregated.Should().Be(2);
        aggregate.Aggregates[_ratingQuestion.Id].AnswersAggregated.Should().Be(1);
        aggregate.Aggregates[_numberQuestion.Id].AnswersAggregated.Should().Be(2);
        aggregate.Aggregates[_singleSelectQuestion.Id].AnswersAggregated.Should().Be(2);
        aggregate.Aggregates[_multiSelectQuestion.Id].AnswersAggregated.Should().Be(2);
    }
}
