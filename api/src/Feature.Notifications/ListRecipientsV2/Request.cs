using Feature.Notifications.RecipientsV2;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.Notifications.ListRecipientsV2;

public class Request : BaseFilterConditionsSortPaginatedRequest, IRecipientsV2Filters
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}
