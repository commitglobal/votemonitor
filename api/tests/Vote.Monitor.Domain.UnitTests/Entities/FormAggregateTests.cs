using System.Text.Json;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

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
        var regularOptionId = Guid.NewGuid();
        string[] languages = ["EN"];

        List<SelectOption> singleSelectOptions =
        [
            SelectOption.Create(flaggedOptionId1, new TranslatedStringFaker(languages).Generate(), false, true),
            SelectOption.Create(Guid.NewGuid(), new TranslatedStringFaker(languages).Generate()),
            SelectOption.Create(Guid.NewGuid(), new TranslatedStringFaker(languages).Generate()),
            SelectOption.Create(Guid.NewGuid(), new TranslatedStringFaker(languages).Generate()),
        ];

        List<SelectOption> multiSelectOptions =
        [
            SelectOption.Create(flaggedOptionId1, new TranslatedStringFaker(languages).Generate(), false, true),
            SelectOption.Create(Guid.NewGuid(), new TranslatedStringFaker(languages).Generate()),
            SelectOption.Create(regularOptionId, new TranslatedStringFaker(languages).Generate()),
            SelectOption.Create(flaggedOptionId2, new TranslatedStringFaker(languages).Generate(), false, true),
        ];

        var singleSelectQuestion =
            new SingleSelectQuestionFaker(singleSelectOptions, languageList: languages).Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker(multiSelectOptions, languageList: languages).Generate();

        var questions = new BaseQuestion[]
        {
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "",
            new TranslatedStringFaker(languages).Generate(), new TranslatedStringFaker(languages).Generate(), "EN",
            languages, questions);

        // Act

        List<BaseAnswer> answers =
        [
            new SingleSelectAnswer(singleSelectQuestion.Id, SelectedOption.Create(flaggedOptionId1, "")),
            new MultiSelectAnswer(multiSelectQuestion.Id, [
                SelectedOption.Create(flaggedOptionId1, ""),
                SelectedOption.Create(flaggedOptionId2, ""),
                SelectedOption.Create(regularOptionId, ""),
            ]),
        ];

        var submission = form.CreateFormSubmission(pollingStation, monitoringObserver, answers, false);

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
        var regularOptionId = Guid.NewGuid();
        string[] languages = ["EN"];

        List<SelectOption> singleSelectOptions =
        [
            SelectOption.Create(flaggedOptionId1, new TranslatedStringFaker(languages).Generate(), false, true),
            SelectOption.Create(Guid.NewGuid(), new TranslatedStringFaker(languages).Generate()),
            SelectOption.Create(Guid.NewGuid(), new TranslatedStringFaker(languages).Generate()),
            SelectOption.Create(Guid.NewGuid(), new TranslatedStringFaker(languages).Generate()),
        ];

        List<SelectOption> multiSelectOptions =
        [
            SelectOption.Create(flaggedOptionId1, new TranslatedStringFaker(languages).Generate(), false, true),
            SelectOption.Create(Guid.NewGuid(), new TranslatedStringFaker(languages).Generate()),
            SelectOption.Create(regularOptionId, new TranslatedStringFaker(languages).Generate()),
            SelectOption.Create(flaggedOptionId2, new TranslatedStringFaker(languages).Generate(), false, true),
        ];

        var singleSelectQuestion =
            new SingleSelectQuestionFaker(singleSelectOptions, languageList: languages).Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker(multiSelectOptions, languageList: languages).Generate();

        var questions = new BaseQuestion[]
        {
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "",
            new TranslatedStringFaker(languages).Generate(), new TranslatedStringFaker(languages).Generate(), "EN",
            languages, questions);

        List<BaseAnswer> initialAnswers =
        [
            new SingleSelectAnswer(singleSelectQuestion.Id, SelectedOption.Create(flaggedOptionId1, ""))
        ];

        var submission = form.CreateFormSubmission(pollingStation, monitoringObserver, initialAnswers, false);

        // Act
        List<BaseAnswer> updatedAnswers =
        [
            new SingleSelectAnswer(singleSelectQuestion.Id, SelectedOption.Create(flaggedOptionId1, "")),
            new MultiSelectAnswer(multiSelectQuestion.Id, [
                SelectedOption.Create(flaggedOptionId1, ""),
                SelectedOption.Create(flaggedOptionId2, ""),
                SelectedOption.Create(regularOptionId, ""),
            ]),
        ];

        form.FillIn(submission, updatedAnswers, false);

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
        string[] languages = ["EN"];

        var textQuestion = new TextQuestionFaker(languageList: languages).Generate();
        var dateQuestion = new DateQuestionFaker(languageList: languages).Generate();
        var ratingQuestion = new RatingQuestionFaker(languageList: languages).Generate();
        var numberQuestion = new NumberQuestionFaker(languageList: languages).Generate();
        var singleSelectQuestion = new SingleSelectQuestionFaker(languageList: languages).Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker(languageList: languages).Generate();

        var questions = new BaseQuestion[]
        {
            textQuestion,
            dateQuestion,
            ratingQuestion,
            numberQuestion,
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "",
            new TranslatedStringFaker(languages).Generate(), new TranslatedStringFaker(languages).Generate(), "EN",
            languages, questions);

        List<BaseAnswer> answers =
        [
            new SingleSelectAnswerFaker(singleSelectQuestion),
            new MultiSelectAnswerFaker(multiSelectQuestion),
            new TextAnswerFaker(textQuestion.Id),
            new NumberAnswerFaker(numberQuestion.Id),
            new RatingAnswerFaker(ratingQuestion.Id),
            new DateAnswerFaker(dateQuestion.Id),
        ];

        // Act
        var submission = form.CreateFormSubmission(pollingStation, monitoringObserver, answers, false);

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
        string[] languages = ["EN"];

        var textQuestion = new TextQuestionFaker(languageList: languages).Generate();
        var dateQuestion = new DateQuestionFaker(languageList: languages).Generate();
        var ratingQuestion = new RatingQuestionFaker(languageList: languages).Generate();
        var numberQuestion = new NumberQuestionFaker(languageList: languages).Generate();
        var singleSelectQuestion = new SingleSelectQuestionFaker(languageList: languages).Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker(languageList: languages).Generate();

        var questions = new BaseQuestion[]
        {
            textQuestion,
            dateQuestion,
            ratingQuestion,
            numberQuestion,
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "",
            new TranslatedStringFaker(languages).Generate(), new TranslatedStringFaker(languages).Generate(), "EN",
            ["EN"], questions);

        List<BaseAnswer> initialAnswers =
        [
            new SingleSelectAnswerFaker(singleSelectQuestion)
        ];

        var submission = form.CreateFormSubmission(pollingStation, monitoringObserver, initialAnswers, false);

        // Act
        List<BaseAnswer> updatedAnswers =
        [
            new SingleSelectAnswerFaker(singleSelectQuestion),
            new MultiSelectAnswerFaker(multiSelectQuestion),
            new TextAnswerFaker(textQuestion.Id),
            new NumberAnswerFaker(numberQuestion.Id),
            new RatingAnswerFaker(ratingQuestion.Id),
            new DateAnswerFaker(dateQuestion.Id),
        ];

        form.FillIn(submission, updatedAnswers, false);

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
        string[] languages = ["EN"];

        var textQuestion = new TextQuestionFaker(languageList: languages).Generate();
        var dateQuestion = new DateQuestionFaker(languageList: languages).Generate();
        var ratingQuestion = new RatingQuestionFaker(languageList: languages).Generate();
        var numberQuestion = new NumberQuestionFaker(languageList: languages).Generate();
        var singleSelectQuestion = new SingleSelectQuestionFaker(languageList: languages).Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker(languageList: languages).Generate();

        var questions = new BaseQuestion[]
        {
            textQuestion,
            dateQuestion,
            ratingQuestion,
            numberQuestion,
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "",
            new TranslatedStringFaker(languages).Generate(), new TranslatedStringFaker(languages).Generate(), "EN",
            languages, questions);

        List<BaseAnswer> initialAnswers =
        [
            new SingleSelectAnswerFaker(singleSelectQuestion),
            new MultiSelectAnswerFaker(multiSelectQuestion),
            new TextAnswerFaker(textQuestion.Id),
            new NumberAnswerFaker(numberQuestion.Id),
            new RatingAnswerFaker(ratingQuestion.Id),
            new DateAnswerFaker(dateQuestion.Id),
        ];

        var submission = form.CreateFormSubmission(pollingStation, monitoringObserver, initialAnswers, false);

        // Act
        form.FillIn(submission, [], false);

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

        string[] languages = ["EN"];
        var textQuestion = new TextQuestionFaker(languages).Generate();
        var dateQuestion = new DateQuestionFaker(languages).Generate();
        var ratingQuestion = new RatingQuestionFaker(languageList: languages).Generate();
        var numberQuestion = new NumberQuestionFaker(languages).Generate();
        var singleSelectQuestion = new SingleSelectQuestionFaker(languageList: languages).Generate();
        var multiSelectQuestion = new MultiSelectQuestionFaker(languageList: languages).Generate();

        var questions = new BaseQuestion[]
        {
            textQuestion,
            dateQuestion,
            ratingQuestion,
            numberQuestion,
            singleSelectQuestion,
            multiSelectQuestion,
        };

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "",
            new TranslatedStringFaker(languages).Generate(), new TranslatedStringFaker(languages).Generate(), "EN",
            ["EN"], questions);
        List<BaseAnswer> initialAnswers =
        [
            new SingleSelectAnswerFaker(singleSelectQuestion),
            new MultiSelectAnswerFaker(multiSelectQuestion),
            new TextAnswerFaker(textQuestion.Id),
            new NumberAnswerFaker(numberQuestion.Id),
            new RatingAnswerFaker(ratingQuestion.Id),
            new DateAnswerFaker(dateQuestion.Id),
        ];

        var submission = form.CreateFormSubmission(pollingStation, monitoringObserver, initialAnswers, false);

        // Act
        form.FillIn(submission, null, false);

        // Assert
        submission.Answers.Should().HaveCount(6);
    }

    [Fact]
    public void WhenOptionIdsAreNotUniqueInForm_ShouldComputeNumberOfFlaggedAnswers_Correctly()
    {
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker().Generate();
        var pollingStation = new PollingStationFaker().Generate();
        var monitoringObserver = new MonitoringObserverFaker().Generate();
        string[] languages = ["RO"];

        var questions = JsonSerializer.Deserialize<List<BaseQuestion>>(FormTestData.QuestionsJson);

        var answers = JsonSerializer.Deserialize<List<BaseAnswer>>(FormTestData.AnswersJson);

        var form = Form.Create(electionRound, monitoringNgo, FormType.ClosingAndCounting, "",
            new TranslatedStringFaker(languages).Generate(), new TranslatedStringFaker(languages).Generate(), "RO",
            languages, questions!);

        // Act
        var submission = form.CreateFormSubmission(pollingStation, monitoringObserver, answers, false);

        // Assert
        submission.NumberOfFlaggedAnswers.Should().Be(1);
    }
}