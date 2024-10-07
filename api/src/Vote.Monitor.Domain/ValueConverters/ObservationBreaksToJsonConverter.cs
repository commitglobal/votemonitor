using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Domain.ValueConverters;

public class ObservationBreaksToJsonConverter : ValueConverter<IReadOnlyList<ObservationBreak>, string>
{
    public ObservationBreaksToJsonConverter() : base(
        v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true }),
        v => JsonSerializer.Deserialize<IReadOnlyList<ObservationBreak>>(v, new JsonSerializerOptions { WriteIndented = true }))
    {
    }
}