using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.Api.Feature.FormTemplate.Create;

public class Request
{
    public string Code { get; set; }
    public TranslatedString Name { get; set; }
    public FormType FormType { get; set; }
    public List<string> Languages { get; set; }
}
