namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public class MultiSelectAnswer : BaseAnswer
{
    public List<SelectedOption> Selection { get; set; }
}
