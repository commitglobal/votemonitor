using Vote.Monitor.Answer.Module.Requests;
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
    public bool IsCompleted { get; set; }
}