using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Feature.DataExport.Start;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public ExportedDataType ExportedDataType { get; set; }
    
    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }
    
    public FormSubmissionsFilters? FormSubmissionsFilters { get; set; }
    public QuickReportsFilters? QuickReportsFilters { get; set; }
    public CitizenReportsFilters? CitizenReportsFilters { get; set; }
    public IncidentReportsFilters? IncidentReportsFilters { get; set; }
}
