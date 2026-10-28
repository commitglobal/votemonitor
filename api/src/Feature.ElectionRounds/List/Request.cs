using Vote.Monitor.Core.Models;

namespace Feature.ElectionRounds.List;

public class Request : BaseSortPaginatedRequest
{
    [QueryParam] public string? SearchText { get; set; }

    /// <summary>Only rounds in any of these countries (repeat the key for several).</summary>
    [QueryParam] public Guid[]? CountryId { get; set; }

    /// <summary>Only rounds with any of these statuses (repeat the key for several).</summary>
    [QueryParam] public ElectionRoundStatus[]? ElectionRoundStatus { get; set; }
}
