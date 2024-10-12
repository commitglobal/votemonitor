using Vote.Monitor.Core.Security;

namespace Feature.IncidentReports.SetCompletion;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid Id { get; set; }
    public bool IsCompleted { get; set; }
}
