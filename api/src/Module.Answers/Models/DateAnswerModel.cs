using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Module.Answers.Models;

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
