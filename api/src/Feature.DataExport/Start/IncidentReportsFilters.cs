using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.DataExport.Start;

public record IncidentReportsFilters
{
    public Guid NgoId { get; init; }
    public DataSource DataSource { get; init; } = DataSource.Ngo;
    public Guid? CoalitionMemberId { get; init; }
    public string? SearchText { get; init; }

    public string? Level1Filter { get; init; }

    public string? Level2Filter { get; init; }

    public string? Level3Filter { get; init; }

    public string? Level4Filter { get; init; }

    public string? Level5Filter { get; init; }

    public string? PollingStationNumberFilter { get; init; }

    public bool? HasFlaggedAnswers { get; init; }

    public Guid? MonitoringObserverId { get; init; }

    public string[]? TagsFilter { get; init; } = [];

    public MonitoringObserverStatus? MonitoringObserverStatus { get; init; }
    public Guid? FormId { get; init; }
    public bool? HasNotes { get; init; }
    public bool? HasAttachments { get; init; }
    public QuestionsAnsweredFilter? QuestionsAnswered { get; init; }
    public IncidentReportFollowUpStatus? FollowUpStatus { get; init; }
    public IncidentReportLocationType? LocationType { get; init; }

    public DateTime? FromDateFilter { get; init; }
    public DateTime? ToDateFilter { get; init; }
    public bool? IsCompletedFilter { get; init; }

    public ExportIncidentReportsFilters ToFilter()
    {
        return new ExportIncidentReportsFilters
        {
            NgoId = NgoId,
            SearchText = SearchText,
            Level1Filter = Level1Filter,
            Level2Filter = Level2Filter,
            Level3Filter = Level3Filter,
            Level4Filter = Level4Filter,
            Level5Filter = Level5Filter,
            PollingStationNumberFilter = PollingStationNumberFilter,
            HasFlaggedAnswers = HasFlaggedAnswers,
            MonitoringObserverId = MonitoringObserverId,
            TagsFilter = TagsFilter,
            MonitoringObserverStatus = MonitoringObserverStatus,
            FormId = FormId,
            HasNotes = HasNotes,
            HasAttachments = HasAttachments,
            QuestionsAnswered = QuestionsAnswered,
            FollowUpStatus = FollowUpStatus,
            LocationType = LocationType,
            FromDateFilter = FromDateFilter,
            ToDateFilter = ToDateFilter,
            IsCompletedFilter = IsCompletedFilter,
            DataSource = DataSource,
            CoalitionMemberId = CoalitionMemberId
        };
    }
}
