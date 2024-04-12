using Vote.Monitor.Core.Models;

namespace Feature.Form.Submissions.List;

public class Request: BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim("NgoId")]
    public Guid NgoId { get; set; }
}
