using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.SetCompletion;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid PollingStationId { get; set; }
    public Guid FormId { get; set; }
    public bool IsCompleted { get; set; }
}
