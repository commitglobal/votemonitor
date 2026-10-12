using Vote.Monitor.Core.Security;

namespace Feature.PollingStation.Information.GetById;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid Id { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId, IsRequired = false)]
    public Guid? NgoId { get; set; }
}
