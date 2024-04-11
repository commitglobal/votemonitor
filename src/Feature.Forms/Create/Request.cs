using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid MonitoringNgoId { get; set; }
    public string Code { get; set; }
    public TranslatedString Name { get; set; }
    public FormType FormType { get; set; }
    public List<string> Languages { get; set; } = [];
    public string DefaultLanguage { get; set; }
}
