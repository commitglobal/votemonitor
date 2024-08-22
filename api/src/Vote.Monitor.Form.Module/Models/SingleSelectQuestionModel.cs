using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Form.Module.Models;

public class SingleSelectQuestionModel : BaseQuestionModel
{
    public List<SelectOptionModel> Options { get; init; }

    public static SingleSelectQuestionModel FromEntity(SingleSelectQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext,
            Options = question.Options.Select(SelectOptionModel.FromEntity).ToList(),
            DisplayLogic = DisplayLogicModel.FromEntity(question.DisplayLogic)
        };
}
