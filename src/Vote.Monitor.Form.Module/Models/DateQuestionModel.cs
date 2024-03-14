using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Form.Module.Models;

public class DateQuestionModel : BaseQuestionModel
{
    public Guid Id { get; init; }
    public string Code { get; init; }

    public static DateQuestionModel FromEntity(DateQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext
        };
}
