using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.GetExportedData;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
    public string ExportedDataId { get; set; }
}
