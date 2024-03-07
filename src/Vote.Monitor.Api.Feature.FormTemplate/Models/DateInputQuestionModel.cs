using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Models;

public class DateInputQuestionModel : BaseQuestionModel
{
    public Guid Id { get; init; }
    public string Code { get; init; }

    public static DateInputQuestionModel FromEntity(DateInputQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext
        };
}
