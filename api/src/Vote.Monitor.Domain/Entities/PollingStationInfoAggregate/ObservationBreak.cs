using System.Text.Json.Serialization;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

public record ObservationBreak
{

    public DateTime Start { get; }
    public DateTime? End { get; }

    [JsonConstructor]
    private ObservationBreak(DateTime start, DateTime? end)
    {
        Start = start;
        End = end;
    }
    public static ObservationBreak Create(DateTime start, DateTime? end) => new(start, end);
};