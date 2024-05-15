using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.ElectionRound.List;

public class Request : BaseSortPaginatedRequest
{
    [QueryParam]
    public string? TitleFilter { get; set; }

    [QueryParam]
    public Guid? CountryId { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<ElectionRoundStatus, string>))]
    public ElectionRoundStatus? Status { get; set; }
}
