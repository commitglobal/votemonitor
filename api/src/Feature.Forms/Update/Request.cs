using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Form.Module.Requests;

namespace Feature.Forms.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid Id { get; set; }
    public string Code { get; set; }
    public string DefaultLanguage { get; set; }
    public TranslatedString Name { get; set; } = new();
    public TranslatedString Description { get; set; } = new();
    public FormType FormType { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<BaseQuestionRequest> Questions { get; set; } = new();
}
