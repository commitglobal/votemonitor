using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.Models;

public class MultiSelectAnswerModel : BaseAnswerModel
{
    public List<SelectedOptionModel> Selection { get; set; } = [];

    public static MultiSelectAnswerModel FromEntity(MultiSelectAnswer multiSelectAnswer)
    {
        return new MultiSelectAnswerModel
        {
            Selection = multiSelectAnswer.Selection?.Select(SelectedOptionModel.FromEntity).ToList() ?? [],
            QuestionId = multiSelectAnswer.QuestionId
        };
    }
}
