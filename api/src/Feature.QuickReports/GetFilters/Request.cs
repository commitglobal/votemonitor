using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.QuickReports.GetFilters;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public DataSource DataSource { get; set; } = DataSource.Ngo;
}
