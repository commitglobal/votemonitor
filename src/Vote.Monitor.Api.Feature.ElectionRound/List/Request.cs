using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.ElectionRound.List;

public class Request : BaseFilterRequest
{
    [QueryParam]
    public string? NameFilter { get; set; }

    [QueryParam]
    public Guid? CountryId { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<ElectionRoundStatus, string>))]
    public ElectionRoundStatus? Status { get; set; }
}
