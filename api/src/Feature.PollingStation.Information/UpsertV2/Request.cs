using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Core.Security;

namespace Feature.PollingStation.Information.UpsertV2;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public bool? IsCompleted { get; set; }

    public DateTime? ArrivalTime { get; set; }
    public DateTime? DepartureTime { get; set; }

    public List<BaseAnswerRequest>? Answers { get; set; }

    public List<BreakRequest>? Breaks { get; set; }


    public class BreakRequest
    {
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
    }
}