using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Models;

public class NumberInputQuestionModel : BaseQuestionModel
{
    public Guid Id { get; init; }
    public string Code { get; init; }
    public TranslatedString InputPlaceholder { get; init; }

    public static NumberInputQuestionModel FromEntity(NumberInputQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext,
            InputPlaceholder = question.InputPlaceholder
        };
}
