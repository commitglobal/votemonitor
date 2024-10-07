using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.Form.Submissions.Models;

public class ObservationBreakModel
{
    public DateTime End { get; init; }
    public DateTime Start { get; init; }
}