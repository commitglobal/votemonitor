using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;

namespace Feature.IssueReports.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid FormId { get; set; }
    public Guid IssueReportId { get; set; }
    
    public IssueReportLocationType LocationType { get; set; }
    public Guid? PollingStationId { get; set; }
    public string? LocationDescription { get; set; }
    public List<BaseAnswerRequest>? Answers { get; set; }
}