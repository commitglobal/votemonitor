using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

public class ExportFormSubmissionsFilters
{
    public string? SearchText { get; set; }
    public DataSource DataSource { get; set; } = DataSource.Ngo;
    public Guid? CoalitionMemberId { get; set; }
    public FormType? FormTypeFilter { get; set; }
    public string? Level1Filter { get; set; }
    public string? Level2Filter { get; set; }
    public string? Level3Filter { get; set; }
    public string? Level4Filter { get; set; }
    public string? Level5Filter { get; set; }
    public string? PollingStationNumberFilter { get; set; }
    public bool? HasFlaggedAnswers { get; set; }

    public SubmissionFollowUpStatus? FollowUpStatus { get; set; }

    public Guid? MonitoringObserverId { get; set; }
    public string[]? TagsFilter { get; set; } = [];

    public MonitoringObserverStatus? MonitoringObserverStatus { get; set; }

    public Guid? FormId { get; set; }
    public bool? HasNotes { get; set; }
    public bool? HasAttachments { get; set; }

    public QuestionsAnsweredFilter? QuestionsAnswered { get; set; }

    public DateTime? FromDateFilter { get; set; }
    public DateTime? ToDateFilter { get; set; }
    public bool? IsCompletedFilter { get; set; }
}
