using Vote.Monitor.Form.Module.Models;

namespace Vote.Monitor.Api.Feature.FormTemplate;

public record FormTemplateFullModel : FormTemplateSlimModel
{
    public List<BaseQuestionModel> Questions { get; init; } = [];
}
