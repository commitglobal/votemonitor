using Vote.Monitor.Core.Security;

namespace Feature.IncidentReports.GetById;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    /// <summary>The caller's NGO; none for platform admins.</summary>
    [FromClaim(ApplicationClaimTypes.NgoId, IsRequired = false)]
    public Guid? NgoId { get; set; }

    public Guid IncidentReportId { get; set; }
}