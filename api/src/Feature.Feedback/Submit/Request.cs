using Vote.Monitor.Core.Security;

namespace Feature.Feedback.Submit;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public string UserFeedback { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}
