using Vote.Monitor.Core.Security;

namespace Feature.Forms.UpdateDisplayOrder;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public List<FormDisplayOrderModel> Forms { get; set; } = [];
}

public class FormDisplayOrderModel
{
    public Guid FormId { get; set; }
    public int DisplayOrder { get; set; }
}
