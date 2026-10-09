using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.GetAggregatedV2;

public class Request : BaseFilterConditionsRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid FormId { get; set; }

    public DataSource DataSource { get; set; } = DataSource.Ngo;
}
