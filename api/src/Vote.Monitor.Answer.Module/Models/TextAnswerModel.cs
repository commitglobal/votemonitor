using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.Models;

public class TextAnswerModel : BaseAnswerModel
{
    public string Text { get; set; }

    public static TextAnswerModel FromEntity(TextAnswer textAnswer)
    {
        return new TextAnswerModel
        {
            Text = textAnswer.Text,
            QuestionId = textAnswer.QuestionId
        };
    }
}
