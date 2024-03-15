using Vote.Monitor.Answer.Module.Requests;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    public List<BaseAnswerRequest> Answers { get; set; } = [];
}
