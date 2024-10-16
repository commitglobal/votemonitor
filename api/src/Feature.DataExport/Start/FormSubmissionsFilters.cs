using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.DataExport.Start;

public class FormSubmissionsFilters
{
    public string? SearchText { get; set; }

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

    public ExportFormSubmissionsFilters ToFilter()
    {
        return new ExportFormSubmissionsFilters
        {
            SearchText = SearchText,
            FormTypeFilter = FormTypeFilter,
            Level1Filter = Level1Filter,
            Level2Filter = Level2Filter,
            Level3Filter = Level3Filter,
            Level4Filter = Level4Filter,
            Level5Filter = Level5Filter,
            PollingStationNumberFilter = PollingStationNumberFilter,
            HasFlaggedAnswers = HasFlaggedAnswers,
            FollowUpStatus = FollowUpStatus,
            MonitoringObserverId = MonitoringObserverId,
            TagsFilter = TagsFilter,
            MonitoringObserverStatus = MonitoringObserverStatus,
            FormId = FormId,
            HasNotes = HasNotes,
            HasAttachments = HasAttachments,
            QuestionsAnswered = QuestionsAnswered,
            FromDateFilter = FromDateFilter,
            ToDateFilter = ToDateFilter
        };
    }
}