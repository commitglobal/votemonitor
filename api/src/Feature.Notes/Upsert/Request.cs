using Vote.Monitor.Core.Security;

namespace Feature.Notes.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// Temporary made nullable until we release a mobile version that will always send this property.
    /// </summary>
    public DateTime? LastUpdatedAt { get; set; }
}
