using Microsoft.AspNetCore.Mvc;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.GetFilters;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [FromQuery] public DataSource DataSource { get; set; } = DataSource.Ngo;
}
