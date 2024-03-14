namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public class SingleSelectAnswer : BaseAnswer
{
    public SelectedOption Selection { get; set; }
}
