using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.Models;

public class NumberAnswerModel : BaseAnswerModel
{
    public int Value { get; set; }

    public static NumberAnswerModel FromEntity(NumberAnswer numberAnswer)
    {
        return new NumberAnswerModel
        {
            Value = numberAnswer.Value,
            QuestionId = numberAnswer.QuestionId
        };
    }
}
