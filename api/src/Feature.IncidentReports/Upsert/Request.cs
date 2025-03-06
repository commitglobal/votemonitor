using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid FormId { get; set; }
    public Guid Id { get; set; }
    
    public IncidentReportLocationType LocationType { get; set; }
    public Guid? PollingStationId { get; set; }
    public string? LocationDescription { get; set; }
    public List<BaseAnswerRequest>? Answers { get; set; }
    public bool? IsCompleted { get; set; }
    
    /// <summary>
    /// Temporary made nullable until we release a mobile version that will always send this property.
    /// </summary>
    public DateTime? LastUpdatedAt { get; set; }
}
