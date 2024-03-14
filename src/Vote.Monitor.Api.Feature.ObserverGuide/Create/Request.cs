using Microsoft.AspNetCore.Mvc;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Create;

public class Request
{
    public required Guid ElectionRoundId { get; set; }

    public required Guid PollingStationId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    [FromForm]
    public required IFormFile Attachment { get; set; }
}
