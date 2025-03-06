using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.DataExport.Start;

public record QuickReportsFilters
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

    public ExportQuickReportsFilters ToFilter()
    {
        return new ExportQuickReportsFilters
        {
            NgoId = NgoId,
            Level1Filter = Level1Filter,
            Level2Filter = Level2Filter,
            Level3Filter = Level3Filter,
            Level4Filter = Level4Filter,
            Level5Filter = Level5Filter,
            QuickReportFollowUpStatus = QuickReportFollowUpStatus,
            QuickReportLocationType = QuickReportLocationType,
            IncidentCategory = IncidentCategory,
            FromDateFilter = FromDateFilter,
            ToDateFilter = ToDateFilter,
            DataSource = DataSource,
            CoalitionMemberId = CoalitionMemberId
        };
    }
}
