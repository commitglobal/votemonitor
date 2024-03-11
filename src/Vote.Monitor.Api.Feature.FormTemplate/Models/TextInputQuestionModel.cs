using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Models;

public class TextInputQuestionModel : BaseQuestionModel
{
    public Guid Id { get; init; }
    public string Code { get; init; }
    public TranslatedString InputPlaceholder { get; init; }

    public static TextInputQuestionModel FromEntity(TextInputQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            InputPlaceholder = question.InputPlaceholder,
            Helptext = question.Helptext
        };
}
