using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.List;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    [QueryParam] public string? SearchText { get; set; }

    [QueryParam] public DataSource DataSource { get; set; } = DataSource.Ngo;
    [QueryParam] public string? Level1Filter { get; set; }
    [QueryParam] public string? Level2Filter { get; set; }
    [QueryParam] public string? Level3Filter { get; set; }
    [QueryParam] public string? Level4Filter { get; set; }
    [QueryParam] public string? Level5Filter { get; set; }
    [QueryParam] public QuickReportFollowUpStatus? QuickReportFollowUpStatus { get; set; }
    [QueryParam] public QuickReportLocationType? QuickReportLocationType { get; set; }
    [QueryParam] public IncidentCategory? IncidentCategory { get; set; }
    [QueryParam] public DateTime? FromDateFilter { get; set; }
    [QueryParam] public DateTime? ToDateFilter { get; set; }
    [QueryParam] public Guid? CoalitionMemberId { get; set; }
    [QueryParam] public Guid? MonitoringObserverId { get; set; }
    [QueryParam] public bool? HasAttachments { get; set; }
    [QueryParam] public string[]? TagsFilter { get; set; } = [];
}
