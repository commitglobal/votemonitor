using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

namespace Vote.Monitor.Domain.UnitTests.Entities;

public class FormAggregateTests
{
    [Fact]
    public void WhenCreatingSubmission_ShouldCountFlaggedAnswers()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker().Generate();
        var pollingStation = new PollingStationFaker().Generate();
        var monitoringObserver = new MonitoringObserverFaker().Generate();

        var flaggedOptionId1 = Guid.NewGuid();
        var flaggedOptionId2 = Guid.NewGuid();
        var flaggedOptionId3 = Guid.NewGuid();
        var regularOptionId = Guid.NewGuid();

        List<SelectOption> singleSelectQuestions = [
            SelectOption.Create(flaggedOptionId1, new TranslatedString(), false, true),
            SelectOption.Create(Guid.NewGuid(), new TranslatedString(), false, false),
            SelectOption.Create(Guid.NewGuid(), new TranslatedString(), false, false),
            SelectOption.Create(Guid.NewGuid(), new TranslatedString(), false, false),
        ];

        List<SelectOption> multiSelectQuestions = [
            SelectOption.Create(flaggedOptionId2, new TranslatedString(), false, true),
            SelectOption.Create(Guid.NewGuid(), new TranslatedString(), false, false),
            SelectOption.Create(regularOptionId, new TranslatedString(), false, false),
            SelectOption.Create(flaggedOptionId3, new TranslatedString(), false, true),
        ];

        var singleSelectQuestion = new SingleSelectQuestionFaker(singleSelectQuestions).Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker(multiSelectQuestions).Generate();

        var questions = new BaseQuestion[]
        {
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "", new TranslatedString(), new TranslatedString(), "EN", ["EN"], questions);

        // Act

        List<BaseAnswer> answers = [
            new SingleSelectAnswer(singleSelectQuestion.Id, SelectedOption.Create(flaggedOptionId1, "")),
            new MultiSelectAnswer(multiSelectQuestion.Id, [
                SelectedOption.Create(flaggedOptionId2, ""),
                SelectedOption.Create(flaggedOptionId3, ""),
                SelectedOption.Create(regularOptionId, ""),
            ]),
        ];

        var submission = form.CreateFormSubmission(Guid.NewGuid(), pollingStation, monitoringObserver, answers);

        // Assert
        submission.NumberOfFlaggedAnswers.Should().Be(3);
    }

    [Fact]
    public void WhenFillingInSubmission_ShouldUpdateFlaggedAnswers()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker().Generate();
        var pollingStation = new PollingStationFaker().Generate();
        var monitoringObserver = new MonitoringObserverFaker().Generate();

        var flaggedOptionId1 = Guid.NewGuid();
        var flaggedOptionId2 = Guid.NewGuid();
        var flaggedOptionId3 = Guid.NewGuid();
        var regularOptionId = Guid.NewGuid();

        List<SelectOption> singleSelectQuestions = [
            SelectOption.Create(flaggedOptionId1, new TranslatedString(), false, true),
            SelectOption.Create(Guid.NewGuid(), new TranslatedString(), false, false),
            SelectOption.Create(Guid.NewGuid(), new TranslatedString(), false, false),
            SelectOption.Create(Guid.NewGuid(), new TranslatedString(), false, false),
        ];

        List<SelectOption> multiSelectQuestions = [
            SelectOption.Create(flaggedOptionId2, new TranslatedString(), false, true),
            SelectOption.Create(Guid.NewGuid(), new TranslatedString(), false, false),
            SelectOption.Create(regularOptionId, new TranslatedString(), false, false),
            SelectOption.Create(flaggedOptionId3, new TranslatedString(), false, true),
        ];

        var singleSelectQuestion = new SingleSelectQuestionFaker(singleSelectQuestions).Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker(multiSelectQuestions).Generate();

        var questions = new BaseQuestion[]
        {
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "", new TranslatedString(), new TranslatedString(), "EN", ["EN"], questions);

        List<BaseAnswer> initialAnswers = [
            new SingleSelectAnswer(singleSelectQuestion.Id, SelectedOption.Create(flaggedOptionId1, ""))
        ];

        var submission = form.CreateFormSubmission(Guid.NewGuid(), pollingStation, monitoringObserver, initialAnswers);

        // Act
        List<BaseAnswer> updatedAnswers = [
            new SingleSelectAnswer(singleSelectQuestion.Id, SelectedOption.Create(flaggedOptionId1, "")),
            new MultiSelectAnswer(multiSelectQuestion.Id, [
                SelectedOption.Create(flaggedOptionId2, ""),
                SelectedOption.Create(flaggedOptionId3, ""),
                SelectedOption.Create(regularOptionId, ""),
            ]),
        ];

        form.FillIn(submission, updatedAnswers);

        // Assert
        submission.NumberOfFlaggedAnswers.Should().Be(3);
    }

    [Fact]
    public void WhenCreatingSubmission_ShouldCountQuestionsAnswered()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker().Generate();
        var pollingStation = new PollingStationFaker().Generate();
        var monitoringObserver = new MonitoringObserverFaker().Generate();

        var textQuestion = new TextQuestionFaker().Generate();
        var dateQuestion = new DateQuestionFaker().Generate();
        var ratingQuestion = new RatingQuestionFaker().Generate();
        var numberQuestion = new NumberQuestionFaker().Generate();
        var singleSelectQuestion = new SingleSelectQuestionFaker().Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker().Generate();

        var questions = new BaseQuestion[]
        {
            textQuestion,
            dateQuestion,
            ratingQuestion,
            numberQuestion,
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "", new TranslatedString(), new TranslatedString(), "EN", ["EN"], questions);

        List<BaseAnswer> answers = [
            new SingleSelectAnswerFaker(singleSelectQuestion),
            new MultiSelectAnswerFaker(multiSelectQuestion),
            new TextAnswerFaker(textQuestion.Id),
            new NumberAnswerFaker(numberQuestion.Id),
            new RatingAnswerFaker(ratingQuestion.Id),
            new DateAnswerFaker(dateQuestion.Id),
        ];

        // Act
        var submission = form.CreateFormSubmission(Guid.NewGuid(), pollingStation, monitoringObserver, answers);

        // Assert
        submission.NumberOfQuestionsAnswered.Should().Be(6);
    }

    [Fact]
    public void WhenFillingInSubmission_ShouldUpdateQuestionsAnswered()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker().Generate();
        var pollingStation = new PollingStationFaker().Generate();
        var monitoringObserver = new MonitoringObserverFaker().Generate();

        var textQuestion = new TextQuestionFaker().Generate();
        var dateQuestion = new DateQuestionFaker().Generate();
        var ratingQuestion = new RatingQuestionFaker().Generate();
        var numberQuestion = new NumberQuestionFaker().Generate();
        var singleSelectQuestion = new SingleSelectQuestionFaker().Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker().Generate();

        var questions = new BaseQuestion[]
        {
            textQuestion,
            dateQuestion,
            ratingQuestion,
            numberQuestion,
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "", new TranslatedString(), new TranslatedString(), "EN", ["EN"], questions);

        List<BaseAnswer> initialAnswers = [
            new SingleSelectAnswerFaker(singleSelectQuestion)
        ];

        var submission = form.CreateFormSubmission(Guid.NewGuid(), pollingStation, monitoringObserver, initialAnswers);

        // Act
        List<BaseAnswer> updatedAnswers = [
                new SingleSelectAnswerFaker(singleSelectQuestion),
            new MultiSelectAnswerFaker(multiSelectQuestion),
            new TextAnswerFaker(textQuestion.Id),
            new NumberAnswerFaker(numberQuestion.Id),
            new RatingAnswerFaker(ratingQuestion.Id),
            new DateAnswerFaker(dateQuestion.Id),
        ];

        form.FillIn(submission, updatedAnswers);

        // Assert
        submission.NumberOfQuestionsAnswered.Should().Be(6);
    }

    [Fact]
    public void WhenFillingInSubmission_EmptyAnswers_ShouldClearAnswers()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker().Generate();
        var pollingStation = new PollingStationFaker().Generate();
        var monitoringObserver = new MonitoringObserverFaker().Generate();

        var textQuestion = new TextQuestionFaker().Generate();
        var dateQuestion = new DateQuestionFaker().Generate();
        var ratingQuestion = new RatingQuestionFaker().Generate();
        var numberQuestion = new NumberQuestionFaker().Generate();
        var singleSelectQuestion = new SingleSelectQuestionFaker().Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker().Generate();

        var questions = new BaseQuestion[]
        {
            textQuestion,
            dateQuestion,
            ratingQuestion,
            numberQuestion,
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "", new TranslatedString(), new TranslatedString(), "EN", ["EN"], questions);

        List<BaseAnswer> initialAnswers = [
            new SingleSelectAnswerFaker(singleSelectQuestion),
            new MultiSelectAnswerFaker(multiSelectQuestion),
            new TextAnswerFaker(textQuestion.Id),
            new NumberAnswerFaker(numberQuestion.Id),
            new RatingAnswerFaker(ratingQuestion.Id),
            new DateAnswerFaker(dateQuestion.Id),
        ];

        var submission = form.CreateFormSubmission(Guid.NewGuid(), pollingStation, monitoringObserver, initialAnswers);

        // Act
        form.FillIn(submission, []);

        // Assert
        submission.NumberOfQuestionsAnswered.Should().Be(0);
        submission.NumberOfFlaggedAnswers.Should().Be(0);
        submission.Answers.Should().HaveCount(0);
    }

    [Fact]
    public void WhenFillingInSubmission_EmptyAnswers_ShouldStayTheSame()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker().Generate();
        var pollingStation = new PollingStationFaker().Generate();
        var monitoringObserver = new MonitoringObserverFaker().Generate();

        var textQuestion = new TextQuestionFaker().Generate();
        var dateQuestion = new DateQuestionFaker().Generate();
        var ratingQuestion = new RatingQuestionFaker().Generate();
        var numberQuestion = new NumberQuestionFaker().Generate();
        var singleSelectQuestion = new SingleSelectQuestionFaker().Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker().Generate();

        var questions = new BaseQuestion[]
        {
            textQuestion,
            dateQuestion,
            ratingQuestion,
            numberQuestion,
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "", new TranslatedString(), new TranslatedString(), "EN", ["EN"], questions);
        List<BaseAnswer> initialAnswers = [
            new SingleSelectAnswerFaker(singleSelectQuestion),
            new MultiSelectAnswerFaker(multiSelectQuestion),
            new TextAnswerFaker(textQuestion.Id),
            new NumberAnswerFaker(numberQuestion.Id),
            new RatingAnswerFaker(ratingQuestion.Id),
            new DateAnswerFaker(dateQuestion.Id),
        ];

        var submission = form.CreateFormSubmission(Guid.NewGuid(), pollingStation, monitoringObserver, initialAnswers);

        // Act
        form.FillIn(submission, null);

        // Assert
        submission.Answers.Should().HaveCount(6);
    }
}
