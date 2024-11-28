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
        IEnumerable<BaseQuestion> questions) : base(electionRound,
        FormType.PSI,
        "PSI",
        TranslatedString.New(languages, "PSI"),
        TranslatedString.New(languages, ""),
        defaultLanguage,
        languages,
        null,
        questions,
        FormStatus.Published)
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

    public PollingStationInformation CreatePollingStationInformation(
        Guid userId,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        ValueOrUndefined<DateTime?> arrivalTime,
        ValueOrUndefined<DateTime?> departureTime,
        List<BaseAnswer>? answers,
        List<ObservationBreak>? breaks,
        ValueOrUndefined<bool> isCompleted)
    {
        answers ??= [];

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var numberOfQuestionsAnswered = AnswersHelpers.CountNumberOfQuestionsAnswered(Questions, answers);
        var numberOfFlaggedAnswers = AnswersHelpers.CountNumberOfFlaggedAnswers(Questions, answers);

        return PollingStationInformation.Create(userId, ElectionRound, pollingStation, monitoringObserver, this,
            arrivalTime,
            departureTime, answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, breaks, isCompleted);
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStationInformationForm() : base()
    {
    }
#pragma warning restore CS8618
}
