using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public Guid Id { get; set; }
    public QuickReportLocationType QuickReportLocationType { get; set; }
    public QuickReportIssueType IssueType { get; set; } = QuickReportIssueType.A;

    public QuickReportOfficialComplaintFilingStatus OfficialComplaintFilingStatus { get; set; } =
        QuickReportOfficialComplaintFilingStatus.DoesNotApplyOrOther;

    public string Title { get; set; }
    public string Description { get; set; }
    public Guid? PollingStationId { set; get; }
    public string? PollingStationDetails { get; set; }
}