using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Feature.DataExport.Start;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public ExportedDataType ExportedDataType { get; set; }
}
