using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Models;

public class MultiSelectQuestionModel : BaseQuestionModel
{
    public List<SelectOptionModel> Options { get; init; }

    public static MultiSelectQuestionModel FromEntity(MultiSelectQuestion question) =>
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
