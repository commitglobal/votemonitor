using Vote.Monitor.Core.Models;

namespace Feature.FormTemplates.ListAssignedTemplates;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }
}
