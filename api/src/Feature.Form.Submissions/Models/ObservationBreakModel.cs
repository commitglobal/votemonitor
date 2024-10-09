using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.Form.Submissions.Models;

public class ObservationBreakModel
{
    public DateTime Start { get; init; }
    public DateTime? End { get; init; }
}