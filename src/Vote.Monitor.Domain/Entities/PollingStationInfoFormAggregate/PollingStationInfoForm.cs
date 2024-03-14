using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

public class PollingStationInfoForm : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; set; }
    public ElectionRound ElectionRound { get; set; }
    public PollingStationInfoFormStatus Status { get; private set; }
    public IReadOnlyList<string> Languages { get; private set; } = new List<string>().AsReadOnly();
    public IReadOnlyList<FormSection> Sections { get; private set; } = new List<FormSection>().AsReadOnly();

    private PollingStationInfoForm(
        ElectionRound electionRound,
        IEnumerable<string> languages,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Languages = languages.ToList().AsReadOnly();
        Status = PollingStationInfoFormStatus.Drafted;
    }

    public static PollingStationInfoForm Create(
        ElectionRound electionRound,
        IEnumerable<string> languages,
        ITimeProvider timeProvider) =>
        new(electionRound, languages, timeProvider);

    public PublishResult Publish()
    {
        var validator = new PollingStationInfoFormValidator();
        var validationResult = validator.Validate(this);

        if (!validationResult.IsValid)
        {
            return new PublishResult.InvalidFormTemplate(validationResult);
        }
        Status = PollingStationInfoFormStatus.Published;
        return new PublishResult.Published();
    }

    public void UpdateDetails(IEnumerable<string> languages, IEnumerable<FormSection> sections)
    {
        Languages = languages.ToList().AsReadOnly();
        Sections = sections.ToList().AsReadOnly();
    }

    public PollingStationInfo CreatePollingStationInfo(ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        string selectedLanguage,
        ITimeProvider timeProvider)
    {
        return PollingStationInfo.Create(electionRound, pollingStation, monitoringObserver, this, selectedLanguage, timeProvider);
    }

    public PollingStationInfo FillIn(
        PollingStationInfo filledInForm,
        string formLanguage,
        List<BaseAnswer> answers)
    {
        var questions = Sections.SelectMany(x => x.Questions).ToDictionary(x => x.Id);
        var hasUnknownQuestions = answers.Any(x => questions.ContainsKey(x.QuestionId));
        var validationResults = new List<ValidationResult>();

        foreach (var answer in answers)
        {
            switch (answer)
            {
                case DateAnswer dateAnswer:
                    var dateQuestion = questions[answer.QuestionId];
                    validationResults.Add(ValidateAnswer(dateQuestion, dateAnswer));
                    break;
                case MultiSelectAnswer multiSelectAnswer:
                    var multiSelectQuestion = questions[answer.QuestionId];
                    validationResults.Add(ValidateAnswer(multiSelectQuestion, multiSelectAnswer));
                    break;
                case NumberAnswer numberAnswer:
                    var numericQuestion = questions[answer.QuestionId];
                    validationResults.Add(ValidateAnswer(numericQuestion, numberAnswer));
                    break;
                case RatingAnswer ratingAnswer:
                    var ratingQuestion = questions[answer.QuestionId];
                    validationResults.Add(ValidateAnswer(ratingQuestion, ratingAnswer));
                    break;
                case SingleSelectAnswer singleSelectAnswer:
                    var singleSelectQuestion = questions[answer.QuestionId];
                    validationResults.Add(ValidateAnswer(singleSelectQuestion, singleSelectAnswer));
                    break;
                case TextAnswer textAnswer:
                    var textQuestion = questions[answer.QuestionId];
                    validationResults.Add(ValidateAnswer(textQuestion, textAnswer));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(answer));
            }
        }

        filledInForm.UpdateDetails(formLanguage, answers);

        return filledInForm;
    }

    private ValidationResult ValidateAnswer(BaseQuestion question, DateAnswer answer)
    {
        throw new NotImplementedException();
    }
    private ValidationResult ValidateAnswer(BaseQuestion question, MultiSelectAnswer answer)
    {
        throw new NotImplementedException();
    }
    private ValidationResult ValidateAnswer(BaseQuestion question, NumberAnswer answer)
    {
        throw new NotImplementedException();
    }
    private ValidationResult ValidateAnswer(BaseQuestion question, RatingAnswer answer)
    {
        throw new NotImplementedException();
    } private ValidationResult ValidateAnswer(BaseQuestion question, SingleSelectAnswer answer)
    {
        throw new NotImplementedException();
    }private ValidationResult ValidateAnswer(BaseQuestion question, TextAnswer answer)
    {
        throw new NotImplementedException();
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStationInfoForm()
    {

    }
#pragma warning restore CS8618
}
