using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Answer.Module.Aggregators;
public class CitizenReportFormSubmissionsAggregate
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
    public int SubmissionCount { get; private set; }
    public int TotalNumberOfQuestionsAnswered { get; private set; }
    public int TotalNumberOfFlaggedAnswers { get; private set; }
    
    /// <summary>
    /// Aggregated answers per question id
    /// </summary>
    public IReadOnlyDictionary<Guid, BaseAnswerAggregate> Aggregates { get; }

    public CitizenReportFormSubmissionsAggregate(Domain.Entities.FormAggregate.Form form)
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

    public CitizenReportFormSubmissionsAggregate AggregateAnswers(CitizenReport citizenReport)
    {
        SubmissionCount++;
        TotalNumberOfFlaggedAnswers += citizenReport.NumberOfFlaggedAnswers;
        TotalNumberOfQuestionsAnswered += citizenReport.NumberOfQuestionsAnswered;

        foreach (var answer in citizenReport.Answers)
        {
            if (!Aggregates.TryGetValue(answer.QuestionId, out var aggregate))
            {
                continue;
            }

            aggregate.Aggregate(citizenReport.Id, Guid.Empty, answer);
        }

        return this;
    }
}
