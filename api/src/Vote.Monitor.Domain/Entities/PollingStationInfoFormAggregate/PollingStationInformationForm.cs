using FluentValidation;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

public class PollingStationInformationForm : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public string[] Languages { get; private set; }
    public IReadOnlyList<BaseQuestion> Questions { get; private set; } = new List<BaseQuestion>().AsReadOnly();
    public int NumberOfQuestions { get; private set; }

    private PollingStationInformationForm(
        ElectionRound electionRound,
        IEnumerable<string> languages,
        List<BaseQuestion> questions) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Languages = languages.ToArray();
        Questions = questions.ToList().AsReadOnly();
        NumberOfQuestions = Questions.Count;
    }
    private PollingStationInformationForm(
        ElectionRound electionRound,
        IEnumerable<string> languages) : this(electionRound, languages, [])
    {
    }

    public static PollingStationInformationForm Create(
        ElectionRound electionRound,
        IEnumerable<string> languages) =>
        new(electionRound, languages);

    public static PollingStationInformationForm Create(
        ElectionRound electionRound,
        IEnumerable<string> languages,
        List<BaseQuestion> questions) =>
        new(electionRound, languages, questions);

    public void UpdateDetails(IEnumerable<string> languages, IEnumerable<BaseQuestion> questions)
    {
        Languages = languages.ToArray();
        Questions = questions.ToList().AsReadOnly();
        NumberOfQuestions = Questions.Count;
    }

    public PollingStationInformation CreatePollingStationInformation(PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<BaseAnswer>? answers)
    {
        if (answers == null)
        {
            return PollingStationInformation.Create(ElectionRound, pollingStation, monitoringObserver, this, arrivalTime, departureTime, [], 0);
        }

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        var numberOfQuestionsAnswered = CountNumberOfQuestionsAnswered(answers);

        return PollingStationInformation.Create(ElectionRound, pollingStation, monitoringObserver, this, arrivalTime, departureTime, answers, numberOfQuestionsAnswered);
    }

    public PollingStationInformation FillIn(PollingStationInformation filledInForm, List<BaseAnswer>? answers)
    {
        if (answers is null)
        {
            return filledInForm;
        }

        if (!answers.Any())
        {
            filledInForm.ClearAnswers();
            return filledInForm;
        }

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var numberOfQuestionsAnswered = CountNumberOfQuestionsAnswered(answers);

        filledInForm.UpdateAnswers(answers, numberOfQuestionsAnswered);

        return filledInForm;
    }

    private int CountNumberOfQuestionsAnswered(List<BaseAnswer> answers)
    {
        var questionIds = Questions.Select(x => x.Id).ToList();

        return answers.Count(x => questionIds.Contains(x.QuestionId));
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStationInformationForm()
    {

    }
#pragma warning restore CS8618
}
