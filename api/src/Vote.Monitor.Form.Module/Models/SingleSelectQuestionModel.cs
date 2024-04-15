using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Form.Module.Models;

public class SingleSelectQuestionModel : BaseQuestionModel
{
    public Guid Id { get; init; }
    public string Code { get; init; }
    public List<SelectOptionModel> Options { get; init; }

    public static SingleSelectQuestionModel FromEntity(SingleSelectQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext,
            Options = Enumerable.ToList<SelectOptionModel>(question.Options.Select(SelectOptionModel.FromEntity))
        };
}
