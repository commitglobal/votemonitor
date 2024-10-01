using Vote.Monitor.Core.Security;

namespace Feature.IncidentReports.GetById;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid IncidentReportId { get; set; }
}