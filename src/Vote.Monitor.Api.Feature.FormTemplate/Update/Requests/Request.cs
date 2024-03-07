using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class Request
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public TranslatedString Name { get; set; }
    public FormType FormType { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<SectionRequest> Sections { get; set; } = new();
}
