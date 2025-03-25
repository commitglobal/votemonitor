using Vote.Monitor.Core.Security;

namespace Feature.DataExport.Get;

public class Request
{
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }
    public Guid Id { get; set; }
}
