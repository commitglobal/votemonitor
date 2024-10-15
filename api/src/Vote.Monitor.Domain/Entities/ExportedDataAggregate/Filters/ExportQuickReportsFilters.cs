using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

public class ExportQuickReportsFilters
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
}