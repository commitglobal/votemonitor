using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Form.Module.Requests;

namespace Feature.FormTemplates.Update;

public class Request
{
    public Guid Id { get; set; }
    public string DefaultLanguage { get; set; }
    public string Code { get; set; }
    public TranslatedString Name { get; set; }
    public FormTemplateType FormTemplateType { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<BaseQuestionRequest> Questions { get; set; } = new();
}
