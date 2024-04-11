using Vote.Monitor.Form.Module.Models;

namespace Feature.FormTemplates;

public record FormTemplateFullModel : FormTemplateSlimModel
{
    public List<BaseQuestionModel> Questions { get; init; } = [];
}
