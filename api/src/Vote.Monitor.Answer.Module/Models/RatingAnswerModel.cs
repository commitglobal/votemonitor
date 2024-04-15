using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.Models;

public class RatingAnswerModel : BaseAnswerModel
{
    public int Value { get; set; }

    public static RatingAnswerModel FromEntity(RatingAnswer ratingAnswer)
    {
        return new RatingAnswerModel
        {
            Value = ratingAnswer.Value,
            QuestionId = ratingAnswer.QuestionId
        };
    }
}
