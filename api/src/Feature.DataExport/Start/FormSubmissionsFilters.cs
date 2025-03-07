using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.DataExport.Start;

public record FormSubmissionsFilters
{
    public Guid NgoId { get; init; }
    public DataSource DataSource { get; init; } = DataSource.Ngo;
    public string? SearchText { get; init; }
    public Guid? CoalitionMemberId { get; init; }
    public FormType? FormTypeFilter { get; init; }

    public string? Level1Filter { get; init; }

    public string? Level2Filter { get; init; }

    public string? Level3Filter { get; init; }

    public string? Level4Filter { get; init; }

    public string? Level5Filter { get; init; }

    public string? PollingStationNumberFilter { get; init; }

    public bool? HasFlaggedAnswers { get; init; }

    public SubmissionFollowUpStatus? FollowUpStatus { get; init; }

    public Guid? MonitoringObserverId { get; init; }

    public string[]? TagsFilter { get; init; } = [];

    public MonitoringObserverStatus? MonitoringObserverStatus { get; init; }
    public Guid? FormId { get; init; }
    public bool? HasNotes { get; init; }
    public bool? HasAttachments { get; init; }
    public QuestionsAnsweredFilter? QuestionsAnswered { get; init; }
    public DateTime? FromDateFilter { get; init; }
    public DateTime? ToDateFilter { get; init; }

    public bool? IsCompletedFilter { get; init; }

    public ExportFormSubmissionsFilters ToFilter()
    {
        return new ExportFormSubmissionsFilters
        {
            NgoId = NgoId,
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
            ToDateFilter = ToDateFilter,
            IsCompletedFilter = IsCompletedFilter,
            DataSource = DataSource,
            CoalitionMemberId = CoalitionMemberId
        };
    }
}
