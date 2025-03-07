using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.Notifications.Send;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    public string Title { get; set; }
    public string Body { get; set; }

    public string? SearchText { get; set; }

    public FormType? FormTypeFilter { get; set; }

    public string? Level1Filter { get; set; }

    public string? Level2Filter { get; set; }

    public string? Level3Filter { get; set; }

    public string? Level4Filter { get; set; }

    public string? Level5Filter { get; set; }

    public string? PollingStationNumberFilter { get; set; }

    public bool? HasFlaggedAnswers { get; set; }

    public SubmissionFollowUpStatus? SubmissionsFollowUpStatus { get; set; }

    public string[]? TagsFilter { get; set; } = [];

    public MonitoringObserverStatus? MonitoringObserverStatus { get; set; }
    public Guid? FormId { get; set; }
    public bool? HasNotes { get; set; }
    public bool? HasAttachments { get; set; }
    public QuestionsAnsweredFilter? QuestionsAnswered { get; set; }
    public DateTime? FromDateFilter { get; set; }
    public DateTime? ToDateFilter { get; set; }
    public QuickReportFollowUpStatus? QuickReportFollowUpStatus { get; set; }
    public IncidentCategory? QuickReportIncidentCategory { get; set; }
    public bool? HasQuickReports { get; set; }
}
