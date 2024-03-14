using Microsoft.AspNetCore.Mvc;

namespace Vote.Monitor.Api.Feature.Answers.Attachments.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    [FromForm]
    public IFormFile Attachment { get; set; }
}
