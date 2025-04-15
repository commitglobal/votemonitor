using Vote.Monitor.Core.Models;

namespace Feature.ElectionRounds.List;

public class Request : BaseSortPaginatedRequest
{
    [QueryParam] public string? SearchText { get; set; }

    [QueryParam] public Guid? CountryId { get; set; }

    [QueryParam] public ElectionRoundStatus? ElectionRoundStatus { get; set; }
}
