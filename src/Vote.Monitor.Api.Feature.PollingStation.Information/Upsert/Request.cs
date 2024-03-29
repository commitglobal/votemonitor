using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public List<BaseAnswerRequest> Answers { get; set; } = [];
}
