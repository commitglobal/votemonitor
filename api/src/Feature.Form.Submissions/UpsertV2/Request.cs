using Module.Answers.Requests;
using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.UpsertV2;

public class Request
{
    public Guid Id { get; set; }
    public Guid ElectionRoundId { get; set; }
    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public Guid FormId { get; set; }

    public List<BaseAnswerRequest>? Answers { get; set; }
    public bool? IsCompleted { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
