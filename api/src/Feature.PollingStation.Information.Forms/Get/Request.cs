using Vote.Monitor.Core.Security;

namespace Feature.PollingStation.Information.Forms.Get;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    /// <summary>Set for NGO users only: platform admins and observers read the form without a submission count.</summary>
    [FromClaim(ApplicationClaimTypes.NgoId, isRequired: false)]
    public Guid? NgoId { get; set; }
}
