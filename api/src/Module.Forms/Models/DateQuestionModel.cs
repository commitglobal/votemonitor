using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Models;

public class DateQuestionModel : BaseQuestionModel
{
    public static DateQuestionModel FromEntity(DateQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext,
            DisplayLogic = DisplayLogicModel.FromEntity(question.DisplayLogic)
        };
}
