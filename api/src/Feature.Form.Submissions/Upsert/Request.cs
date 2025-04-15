using Module.Answers.Requests;
using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public Guid FormId { get; set; }

    public List<BaseAnswerRequest>? Answers { get; set; }
    public bool? IsCompleted { get; set; }
    
    /// <summary>
    /// Temporary made nullable until we release a mobile version that will always send this property.
    /// </summary>
    public DateTime? LastUpdatedAt { get; set; }
}
