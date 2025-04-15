using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Feature.Ngos.List;

public class Request : BaseSortPaginatedRequest
{
    [QueryParam] public string? SearchText { get; set; }

    [QueryParam] public NgoStatus? Status { get; set; }
}
