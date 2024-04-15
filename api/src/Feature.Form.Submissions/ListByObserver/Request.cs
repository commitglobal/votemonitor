using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Form.Submissions.ListByObserver;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim("NgoId")]
    public Guid NgoId { get; set; }

    [QueryParam] 
    public string[]? TagsFilter { get; set; } = [];
}
