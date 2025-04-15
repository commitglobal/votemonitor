using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Module.Answers.Models;

public class SingleSelectAnswerModel : BaseAnswerModel
{
    public SelectedOptionModel Selection { get; set; }

    public static SingleSelectAnswerModel FromEntity(SingleSelectAnswer singleSelectAnswer)
    {
        return new SingleSelectAnswerModel
        {
            Selection = SelectedOptionModel.FromEntity(singleSelectAnswer.Selection),
            QuestionId = singleSelectAnswer.QuestionId
        };
    }
}
