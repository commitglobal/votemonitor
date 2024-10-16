using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.DataExport.Start;

public class IncidentReportsFilters
{
    public string? SearchText { get; set; }

    public string? Level1Filter { get; set; }

    public string? Level2Filter { get; set; }

    public string? Level3Filter { get; set; }

    public string? Level4Filter { get; set; }

    public string? Level5Filter { get; set; }

    public string? PollingStationNumberFilter { get; set; }

    public bool? HasFlaggedAnswers { get; set; }

    public Guid? MonitoringObserverId { get; set; }

    public string[]? TagsFilter { get; set; } = [];

    public MonitoringObserverStatus? MonitoringObserverStatus { get; set; }
    public Guid? FormId { get; set; }
    public bool? HasNotes { get; set; }
    public bool? HasAttachments { get; set; }
    public QuestionsAnsweredFilter? QuestionsAnswered { get; set; }
    public IncidentReportFollowUpStatus? FollowUpStatus { get; set; }
    public IncidentReportLocationType? LocationType { get; set; }

    public DateTime? FromDateFilter { get; set; }
    public DateTime? ToDateFilter { get; set; }

    public ExportIncidentReportsFilters ToFilter()
    {
        return new ExportIncidentReportsFilters
        {
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
            ToDateFilter = ToDateFilter
        };
    }
}