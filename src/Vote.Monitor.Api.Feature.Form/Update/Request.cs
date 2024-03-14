using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Api.Feature.Form.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid MonitoringNgoId { get; set; }
    public Guid Id { get; set; }
    public string Code { get; set; }
    public TranslatedString Name { get; set; }
    public FormType FormType { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<SectionRequest> Sections { get; set; } = new();
}
