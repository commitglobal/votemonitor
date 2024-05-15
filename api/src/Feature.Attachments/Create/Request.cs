using Vote.Monitor.Core.Security;

namespace Feature.Attachments.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }

    public IFormFile Attachment { get; set; }
}
