using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information;

public class ObservationBreakModel
{

    public DateTime Start { get; private init; }
    
    public DateTime? End { get; private init; }

    public double? Duration => End?.Subtract(Start).TotalMinutes;
    
    public static ObservationBreakModel FromEntity(ObservationBreak observationBreak)
    {
        return new ObservationBreakModel
        {
            End = observationBreak.End,
            Start = observationBreak.Start
        };
    }
}