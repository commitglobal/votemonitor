using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Module.Answers.Aggregators;
public class CitizenReportFormSubmissionsAggregate
{
    public Guid ElectionRoundId { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public Guid FormId { get; private set; }
    public string FormCode { get; private set; } = null!;
    public FormType FormType { get; private set; } = null!;
    public TranslatedString Name { get; private set; } = null!;
    public TranslatedString Description { get; private set; } = null!;
    public string DefaultLanguage { get; private set; } = null!;
    public string[] Languages { get; private set; } = [];
    public int SubmissionCount { get; private set; }
    public int TotalNumberOfQuestionsAnswered { get; private set; }
    public int TotalNumberOfFlaggedAnswers { get; private set; }
    
    /// <summary>
    /// Aggregated answers per question id
    /// </summary>
    public IReadOnlyDictionary<Guid, BaseAnswerAggregate> Aggregates { get; private set; } = null!;

    public NoteModel[] Notes { get; set; } = [];
    public AggregatedSubmissionsAttachmentModel[] Attachments { get; set; } = [];

    /// <summary>
    /// Used by Dapper when reading an aggregate computed by the database.
    /// </summary>
    private CitizenReportFormSubmissionsAggregate()
    {
    }

    public CitizenReportFormSubmissionsAggregate(Form form)
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

            aggregate.Aggregate(citizenReport.Id, Guid.Empty, answer, locationId: citizenReport.LocationId);
        }

        return this;
    }
}
