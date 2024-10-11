using FluentValidation;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

public class PollingStationInformationForm : BaseForm
{
    private PollingStationInformationForm(
        ElectionRound electionRound,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) : base(electionRound, FormType.PSI, "PSI",
        TranslatedString.New(languages, "PSI"),
        TranslatedString.New(languages, ""), defaultLanguage, languages, questions)
    {
    }

    private PollingStationInformationForm(
        ElectionRound electionRound,
        string defaultLanguage,
        IEnumerable<string> languages) : this(electionRound, defaultLanguage, languages, [])
    {
    }

    public static PollingStationInformationForm Create(
        ElectionRound electionRound,
        string defaultLanguage,
        IEnumerable<string> languages) =>
        new(electionRound, defaultLanguage, languages);

    public static PollingStationInformationForm Create(
        ElectionRound electionRound,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) =>
        new(electionRound, defaultLanguage, languages, questions);

    [Obsolete("Will be removed after 27.10.2024")]
    public PollingStationInformation CreatePollingStationInformation(
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<BaseAnswer>? answers,
        List<ObservationBreak>? breaks,
        bool? isCompleted)
    {
        if (answers == null)
        {
            return PollingStationInformation.Create(ElectionRound, pollingStation, monitoringObserver, this,
                arrivalTime, departureTime, [], 0, 0, breaks, isCompleted);
        }

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var numberOfQuestionsAnswered = AnswersHelpers.CountNumberOfQuestionsAnswered(Questions, answers);
        var numberOfFlaggedAnswers = AnswersHelpers.CountNumberOfFlaggedAnswers(Questions, answers);

        return PollingStationInformation.Create(ElectionRound, pollingStation, monitoringObserver, this, arrivalTime,
            departureTime, answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, breaks, isCompleted);
    }

    public PollingStationInformation CreatePollingStationInformationV2(
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<BaseAnswer>? answers,
        List<ObservationBreak>? breaks,
        bool? isCompleted)
    {
        if (answers == null)
        {
            return PollingStationInformation.CreateV2(ElectionRound, pollingStation, monitoringObserver, this,
                arrivalTime, departureTime, [], 0, 0, breaks, isCompleted);
        }

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var numberOfQuestionsAnswered = AnswersHelpers.CountNumberOfQuestionsAnswered(Questions, answers);
        var numberOfFlaggedAnswers = AnswersHelpers.CountNumberOfFlaggedAnswers(Questions, answers);

        return PollingStationInformation.CreateV2(ElectionRound, pollingStation, monitoringObserver, this, arrivalTime,
            departureTime, answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, breaks, isCompleted);
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStationInformationForm() : base()
    {
    }
#pragma warning restore CS8618
}