using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

public record ExportQuickReportsFilters
{
    public Guid NgoId { get; init; }
    public DataSource DataSource { get; init; } = DataSource.Ngo;
    public Guid? CoalitionMemberId { get; init; }
    public string? Level1Filter { get; init; }
    public string? Level2Filter { get; init; }
    public string? Level3Filter { get; init; }
    public string? Level4Filter { get; init; }
    public string? Level5Filter { get; init; }

    public QuickReportFollowUpStatus? QuickReportFollowUpStatus { get; init; }

    public QuickReportLocationType? QuickReportLocationType { get; init; }

    public IncidentCategory? IncidentCategory { get; init; }

    public DateTime? FromDateFilter { get; init; }
    public DateTime? ToDateFilter { get; init; }
}
