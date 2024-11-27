using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Form.Submissions.GetAggregated;

public class Response
{
    public SubmissionsFilterModel SubmissionsFilter { get; set; }
    public FormSubmissionsAggregate SubmissionsAggregate { get; set; }
    public List<AttachmentModel> Attachments { get; set; }
    public List<NoteModel> Notes { get; set; }
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

    public SubmissionFollowUpStatus? FollowUpStatus { get; set; }

    public string[]? TagsFilter { get; set; } = [];

    public MonitoringObserverStatus? MonitoringObserverStatus { get; set; }
    public bool? HasNotes { get; set; }
    public bool? HasAttachments { get; set; }
    public QuestionsAnsweredFilter? QuestionsAnswered { get; set; }
    public bool? IsCompletedFilter { get; set; }
    public DataSource DataSource { get; set; } = DataSource.Ngo;
    public Guid? CoalitionMemberId { get; set; }
}
