using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Core.Security;

namespace Feature.PollingStation.Information.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
    public Guid Id { get; set; }

    public DateTime? ArrivalTime { get; set; }
    public DateTime? DepartureTime { get; set; }

    public List<BaseAnswerRequest>? Answers { get; set; }
}
