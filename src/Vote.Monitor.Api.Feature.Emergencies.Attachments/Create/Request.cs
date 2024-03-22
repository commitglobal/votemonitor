using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Emergencies.Attachments.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid EmergencyId { get; set; }

    [FromClaim(ClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public IFormFile Attachment { get; set; }
}
