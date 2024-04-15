namespace Vote.Monitor.Answer.Module.Requests;

public class SelectedOptionRequest
{
    public Guid OptionId { get; set; }
    public string Text { get; set; }
}
