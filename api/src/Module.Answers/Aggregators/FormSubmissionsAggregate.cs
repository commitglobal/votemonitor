using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Module.Answers.Models;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Module.Answers.Aggregators;

public record Responder(Guid ResponderId, string DisplayName, string Email, string? PhoneNumber);

public class FormSubmissionsAggregate
{
    public Guid FormId { get; private set; }
    public string FormCode { get; private set; } = null!;

    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public FormType FormType { get; private set; } = null!;

    public TranslatedString Name { get; private set; } = null!;
    public TranslatedString Description { get; private set; } = null!;
    public string DefaultLanguage { get; private set; } = null!;
    public string[] Languages { get; private set; } = [];

    private HashSet<Responder> _responders = new();

    public IReadOnlyList<Responder> Responders
    {
        get => _responders.ToList().AsReadOnly();
        private set => _responders = value.ToHashSet();
    }

    public int SubmissionCount { get; private set; }
    public int TotalNumberOfQuestionsAnswered { get; private set; }
    public int TotalNumberOfFlaggedAnswers { get; private set; }
    public int PollingStationsCount { get; private set; }

    /// <summary>
    /// Aggregated answers per question id
    /// </summary>
    public IReadOnlyDictionary<Guid, BaseAnswerAggregate> Aggregates { get; private set; } = null!;

    public NoteModel[] Notes { get; set; } = [];
    public AggregatedSubmissionsAttachmentModel[] Attachments { get; set; } = [];

    /// <summary>
    /// Used by Dapper when reading an aggregate computed by the database.
    /// </summary>
    private FormSubmissionsAggregate()
    {
    }

    public FormSubmissionsAggregate(Form form)
    {
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
        FormId = form.Id;
        FormCode = FormType.PSI;
        FormType = FormType.PSI;
        Name = TranslatedString.New(form.Languages, "PSI");
        Description = TranslatedString.New(form.Languages, "PSI");
        Languages = form.Languages;
        DefaultLanguage = form.DefaultLanguage;

        Aggregates = form
            .Questions
            .Select(AnswerAggregateFactory.Map)
            .ToDictionary(a => a.QuestionId, x => x)
            .AsReadOnly();
    }

    public FormSubmissionsAggregate AggregateAnswers(FormSubmissionView formSubmission)
    {
        _responders.Add(new Responder(formSubmission.MonitoringObserverId, formSubmission.ObserverName,
            formSubmission.Email, formSubmission.PhoneNumber));

        SubmissionCount++;
        TotalNumberOfFlaggedAnswers += formSubmission.NumberOfFlaggedAnswers;
        TotalNumberOfQuestionsAnswered += formSubmission.NumberOfQuestionsAnswered;

        foreach (var answer in formSubmission.Answers)
        {
            if (!Aggregates.TryGetValue(answer.QuestionId, out var aggregate))
            {
                continue;
            }

            aggregate.Aggregate(formSubmission.SubmissionId, formSubmission.MonitoringObserverId, answer);
        }

        return this;
    }
    public FormSubmissionsAggregate AggregateAnswers(FormSubmission formSubmission)
    {
        var observer = formSubmission.MonitoringObserver.Observer.ApplicationUser;
        _responders.Add(new Responder(formSubmission.MonitoringObserverId, observer.DisplayName,
            observer.Email, observer.PhoneNumber));

        SubmissionCount++;
        TotalNumberOfFlaggedAnswers += formSubmission.NumberOfFlaggedAnswers;
        TotalNumberOfQuestionsAnswered += formSubmission.NumberOfQuestionsAnswered;

        foreach (var answer in formSubmission.Answers)
        {
            if (!Aggregates.TryGetValue(answer.QuestionId, out var aggregate))
            {
                continue;
            }

            aggregate.Aggregate(formSubmission.Id, formSubmission.MonitoringObserverId, answer);
        }

        return this;
    }

    public FormSubmissionsAggregate AggregateAnswers(IncidentReport incidentReport)
    {
        var observer = incidentReport.MonitoringObserver.Observer.ApplicationUser;
        _responders.Add(new Responder(incidentReport.MonitoringObserverId, observer.DisplayName,
            observer.Email, observer.PhoneNumber));

        SubmissionCount++;
        TotalNumberOfFlaggedAnswers += incidentReport.NumberOfFlaggedAnswers;
        TotalNumberOfQuestionsAnswered += incidentReport.NumberOfQuestionsAnswered;

        foreach (var answer in incidentReport.Answers)
        {
            if (!Aggregates.TryGetValue(answer.QuestionId, value: out var aggregate))
            {
                continue;
            }

            aggregate.Aggregate(incidentReport.Id, incidentReport.MonitoringObserverId, answer);
        }

        return this;
    }

    public FormSubmissionsAggregate AggregateAnswers(PollingStationInformation formSubmission)
    {
        var observer = formSubmission.MonitoringObserver.Observer.ApplicationUser;
        _responders.Add(new Responder(formSubmission.MonitoringObserverId, observer.DisplayName,
            observer.Email, observer.PhoneNumber));

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
}
