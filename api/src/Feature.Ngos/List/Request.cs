using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Feature.Ngos.List;

public class Request : BaseSortPaginatedRequest
{
    [QueryParam] public string? SearchText { get; set; }

    /// <summary>Only NGOs with any of these statuses (repeat the key for several).</summary>
    [QueryParam] public NgoStatus[]? Status { get; set; }
}
