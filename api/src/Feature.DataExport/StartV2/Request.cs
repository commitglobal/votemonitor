using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Feature.DataExport.StartV2;

public class Request
{
    public ExportedDataType ExportedDataType { get; set; }
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    public DataSource DataSource { get; set; } = DataSource.Ngo;

    public FilterRule? FilterConditions { get; set; }
}
