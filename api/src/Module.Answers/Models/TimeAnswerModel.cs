using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Module.Answers.Models;

public class TimeAnswerModel : BaseAnswerModel
{
    public TimeOnly Time { get; set; }

    public static TimeAnswerModel FromEntity(TimeAnswer timeAnswer)
    {
        return new TimeAnswerModel
        {
            Time = timeAnswer.Time,
            QuestionId = timeAnswer.QuestionId
        };
    }
}
