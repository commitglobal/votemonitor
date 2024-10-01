using Microsoft.AspNetCore.Mvc;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.CitizenGuideAggregate;

namespace Feature.Citizen.Guides.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    
    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public string Title { get; set; }

    public CitizenGuideType GuideType { get; set; }

    [FromForm]
    public IFormFile? Attachment { get; set; }

    public string? WebsiteUrl { get; set; }
    public string? Text { get; set; }
}
