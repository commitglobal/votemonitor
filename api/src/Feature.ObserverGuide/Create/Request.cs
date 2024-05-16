using Microsoft.AspNetCore.Mvc;
using Vote.Monitor.Core.Security;

namespace Feature.ObserverGuide.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    
    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public string Title { get; set; }

    [FromForm]
    public IFormFile? Attachment { get; set; }

    public string? WebsiteUrl { get; set; }
}
