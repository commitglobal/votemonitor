using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Form.Module.Models;

public class NumberQuestionModel : BaseQuestionModel
{
    public Guid Id { get; init; }
    public string Code { get; init; }
    public TranslatedString InputPlaceholder { get; init; }

    public static NumberQuestionModel FromEntity(NumberQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext,
            InputPlaceholder = question.InputPlaceholder
        };
}
