using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Answer.Module.Aggregators;


public record Responder(Guid ResponderId, string FirstName, string LastName, string Email, string PhoneNumber);
public class FormSubmissionsAggregate
{
    public Guid ElectionRoundId { get; }
    public Guid MonitoringNgoId { get; }
    public Guid FormId { get; }
    public string FormCode { get; }
    public FormType FormType { get; }
    public TranslatedString Name { get; }
    public TranslatedString Description { get; }
    public string DefaultLanguage { get; }
    public string[] Languages { get; } = [];

    private readonly HashSet<Responder> _responders = new();
    public IReadOnlyList<Responder> Responders => _responders.ToList().AsReadOnly();

    public int SubmissionCount { get; private set; }
    public int TotalNumberOfQuestionsAnswered { get; private set; }
    public int TotalNumberOfFlaggedAnswers { get; private set; }
    /// <summary>
    /// Aggregated answers per question id
    /// </summary>
    public IReadOnlyDictionary<Guid, BaseAnswerAggregate> Aggregates { get; }

    public FormSubmissionsAggregate(Form form)
    {
        ElectionRoundId = form.ElectionRoundId;
        MonitoringNgoId = form.MonitoringNgoId;
        FormId = form.Id;
        FormCode = form.Code;
        FormType = form.FormType;
        Name = form.Name;
        Description = form.Description;
        Languages = form.Languages;
        DefaultLanguage = form.DefaultLanguage;

        Aggregates = form
            .Questions
            .Select(AnswerAggregateFactory.Map)
            .ToDictionary(a => a.QuestionId, x => x)
            .AsReadOnly();
    }

    public FormSubmissionsAggregate(PollingStationInformationForm form)
    {
        ElectionRoundId = form.ElectionRoundId;
        MonitoringNgoId = Guid.Empty;
        FormId = form.Id;
        FormCode = FormType.PSI;
        FormType = FormType.PSI;
        Name = GetName(form.Languages, "PSI");
        Description = GetName(form.Languages, "PSI");
        Languages = form.Languages;
        DefaultLanguage = form.DefaultLanguage;

        Aggregates = form
            .Questions
            .Select(AnswerAggregateFactory.Map)
            .ToDictionary(a => a.QuestionId, x => x)
            .AsReadOnly();
    }

    private TranslatedString GetName(string[] languages, string value)
    {
        var translatedString = new TranslatedString();
        foreach (var language in languages)
        {
            translatedString.Add(language, value);
        }

        return translatedString;
    }

    public FormSubmissionsAggregate AggregateAnswers(FormSubmission formSubmission)
    {
        var observer = formSubmission.MonitoringObserver.Observer.ApplicationUser;
        _responders.Add(new Responder(formSubmission.MonitoringObserverId, observer.FirstName, observer.LastName, observer.Email, observer.PhoneNumber));

        SubmissionCount++;
        TotalNumberOfFlaggedAnswers += formSubmission.NumberOfFlaggedAnswers;
        TotalNumberOfQuestionsAnswered += formSubmission.NumberOfQuestionsAnswered;

        foreach (var answer in formSubmission.Answers)
        {
            if (!Aggregates.ContainsKey(answer.QuestionId))
            {
                continue;
            }

            Aggregates[answer.QuestionId].Aggregate(formSubmission.Id, formSubmission.MonitoringObserverId, answer);
        }

        return this;
    }
    public FormSubmissionsAggregate AggregateAnswers(PollingStationInformation formSubmission)
    {
        var observer = formSubmission.MonitoringObserver.Observer.ApplicationUser;
        _responders.Add(new Responder(formSubmission.MonitoringObserverId, observer.FirstName, observer.LastName, observer.Email, observer.PhoneNumber));

        SubmissionCount++;
        TotalNumberOfQuestionsAnswered += formSubmission.NumberOfQuestionsAnswered;

        foreach (var answer in formSubmission.Answers)
        {
            Aggregates[answer.QuestionId].Aggregate(formSubmission.Id, formSubmission.MonitoringObserverId, answer);
        }

        return this;
    }
}
