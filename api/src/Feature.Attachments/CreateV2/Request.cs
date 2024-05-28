using Vote.Monitor.Core.Security;

namespace Feature.Attachments.CreateV2;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
}
