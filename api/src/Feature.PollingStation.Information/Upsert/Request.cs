using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.PollingStation.Information.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public ValueOrUndefined<bool> IsCompleted { get; set; }= ValueOrUndefined<bool>.Undefined();
    public ValueOrUndefined<DateTime?> ArrivalTime { get; set; }= ValueOrUndefined<DateTime?>.Undefined();
    public ValueOrUndefined<DateTime?> DepartureTime { get; set; } = ValueOrUndefined<DateTime?>.Undefined();

    public List<BaseAnswerRequest>? Answers { get; set; }

    public List<BreakRequest>? Breaks { get; set; }


    public class BreakRequest
    {
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
    }
}