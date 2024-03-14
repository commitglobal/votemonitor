namespace Vote.Monitor.Api.Feature.Emergencies.Attachments.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid EmergencyId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    public IFormFile Attachment { get; set; }
}
