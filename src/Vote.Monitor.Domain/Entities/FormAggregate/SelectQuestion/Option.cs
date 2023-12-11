namespace Vote.Monitor.Domain.Entities.FormAggregate.SelectQuestion;
public class Option
{

    private Option()
    {

    }
    public Option(string id, string text, bool isFlagged)
    {
        Id = id;
        Text = text;
        IsFlagged = isFlagged;
    }
    public string Id { get; private set; }
    public string Text { get; private set; }
    public bool IsFlagged { get; private set; }


}
