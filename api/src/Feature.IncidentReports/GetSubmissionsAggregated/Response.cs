using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.GetSubmissionsAggregated;

public class Response
{
    public SubmissionsFilterModel SubmissionsFilter { get; set; }
    public FormSubmissionsAggregate SubmissionsAggregate { get; set; }
    public AttachmentModel[] Attachments { get; set; } = [];
    public NoteModel[] Notes { get; set; } = [];

}

public class SubmissionsFilterModel
{
    public string? Level1Filter { get; set; }

    public string? Level2Filter { get; set; }

    public string? Level3Filter { get; set; }

    public string? Level4Filter { get; set; }

    public string? Level5Filter { get; set; }
    public string? PollingStationNumberFilter { get; set; }

    public bool? HasFlaggedAnswers { get; set; }

    public IncidentReportFollowUpStatus? FollowUpStatusFilter { get; set; }
    public IncidentReportLocationType? LocationTypeFilter { get; set; }
    public bool? HasNotes { get; set; }
    public bool? HasAttachments { get; set; }
    public QuestionsAnsweredFilter? QuestionsAnswered { get; set; }
    public bool? IsCompletedFilter { get; set; }
}