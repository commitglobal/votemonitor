using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.Models;

public class DateAnswerModel : BaseAnswerModel
{
    public DateTime Date { get; set; }

    public static DateAnswerModel FromEntity(DateAnswer dateAnswer)
    {
        return new DateAnswerModel
        {
            Date = dateAnswer.Date,
            QuestionId = dateAnswer.QuestionId
        };
    }
}
