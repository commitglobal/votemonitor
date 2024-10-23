using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Form.Submissions.Requests;

public class FormSubmissionsAggregateFilter
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid? FormId { get; set; }

    [QueryParam] public string? Level1Filter { get; set; }

    [QueryParam] public string? Level2Filter { get; set; }

    [QueryParam] public string? Level3Filter { get; set; }

    [QueryParam] public string? Level4Filter { get; set; }

    [QueryParam] public string? Level5Filter { get; set; }

    [QueryParam] public string? PollingStationNumberFilter { get; set; }

    [QueryParam] public bool? HasFlaggedAnswers { get; set; }

    [QueryParam] public SubmissionFollowUpStatus? FollowUpStatus { get; set; }

    [QueryParam] public string[]? TagsFilter { get; set; } = [];

    [QueryParam] public MonitoringObserverStatus? MonitoringObserverStatus { get; set; }
    [QueryParam] public bool? HasNotes { get; set; }
    [QueryParam] public bool? HasAttachments { get; set; }
    [QueryParam] public QuestionsAnsweredFilter? QuestionsAnswered { get; set; }
    [QueryParam] public bool? IsCompletedFilter { get; set; }
}