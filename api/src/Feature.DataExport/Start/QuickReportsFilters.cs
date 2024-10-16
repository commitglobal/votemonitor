using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.DataExport.Start;

public class QuickReportsFilters
{
    public string? Level1Filter { get; set; }
    public string? Level2Filter { get; set; }
    public string? Level3Filter { get; set; }
    public string? Level4Filter { get; set; }
    public string? Level5Filter { get; set; }
    public QuickReportFollowUpStatus? QuickReportFollowUpStatus { get; set; }
    public QuickReportLocationType? QuickReportLocationType { get; set; }
    public IncidentCategory? IncidentCategory { get; set; }
    public DateTime? FromDateFilter { get; set; }
    public DateTime? ToDateFilter { get; set; }

    public ExportQuickReportsFilters ToFilter()
    {
        return new ExportQuickReportsFilters
        {
            Level1Filter = Level1Filter,
            Level2Filter = Level2Filter,
            Level3Filter = Level3Filter,
            Level4Filter = Level4Filter,
            Level5Filter = Level5Filter,
            QuickReportFollowUpStatus = QuickReportFollowUpStatus,
            QuickReportLocationType = QuickReportLocationType,
            IncidentCategory = IncidentCategory,
            FromDateFilter = FromDateFilter,
            ToDateFilter = ToDateFilter
        };

    }
}