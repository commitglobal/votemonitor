using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.Forms.ListV2;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [QueryParam] public string? SearchText { get; set; }

    [QueryParam] public string? LanguageCode { get; set; }

    [QueryParam] public FormStatus? FormStatusFilter { get; set; }

    [QueryParam] public FormType? TypeFilter { get; set; }
}
