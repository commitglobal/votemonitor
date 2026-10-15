using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Models;

public class TimeQuestionModel : BaseQuestionModel
{
    public static TimeQuestionModel FromEntity(TimeQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext,
            DisplayLogic = DisplayLogicModel.FromEntity(question.DisplayLogic)
        };
}
