using Microsoft.AspNetCore.Mvc;
using Vote.Monitor.Core.Security;

namespace Feature.ObserverGuide.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    
    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid MonitoringNgoId { get; set; }
    public string Title { get; set; }

    [FromForm]
    public required IFormFile Attachment { get; set; }
}
