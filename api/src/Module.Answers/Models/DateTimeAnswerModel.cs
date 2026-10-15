using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Module.Answers.Models;

public class DateTimeAnswerModel : BaseAnswerModel
{
    public DateTime DateTime { get; set; }

    public static DateTimeAnswerModel FromEntity(DateTimeAnswer dateTimeAnswer)
    {
        return new DateTimeAnswerModel
        {
            DateTime = dateTimeAnswer.DateTime,
            QuestionId = dateTimeAnswer.QuestionId
        };
    }
}
