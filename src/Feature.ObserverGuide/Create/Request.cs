using Microsoft.AspNetCore.Mvc;

namespace Feature.ObserverGuide.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public string Title { get; set; }

    [FromForm]
    public required IFormFile Attachment { get; set; }
}
