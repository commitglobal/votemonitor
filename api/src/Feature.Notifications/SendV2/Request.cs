using Feature.Notifications.RecipientsV2;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Core.Security;

namespace Feature.Notifications.SendV2;

public class Request : IRecipientsV2Filters
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    public string Title { get; set; }
    public string Body { get; set; }


    public FilterRule? Filter { get; set; }
}
