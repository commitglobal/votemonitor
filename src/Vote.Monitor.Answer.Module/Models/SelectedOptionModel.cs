using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.Models;

public class SelectedOptionModel
{
    public Guid OptionId { get; set; }
    public string Text { get; set; }

    public static SelectedOptionModel FromEntity(SelectedOption selection)
    {
        return new SelectedOptionModel
        {
            Text = selection.Text,
            OptionId = selection.OptionId
        };
    }
}
