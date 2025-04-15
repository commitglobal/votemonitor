using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Models;

public class NumberQuestionModel : BaseQuestionModel
{
    public TranslatedString InputPlaceholder { get; init; }

    public static NumberQuestionModel FromEntity(NumberQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext,
            InputPlaceholder = question.InputPlaceholder,
            DisplayLogic = DisplayLogicModel.FromEntity(question.DisplayLogic)
        };
}
