namespace Vote.Monitor.Domain.Entities.FormAggregate.SelectQuestion;
public class QuestionOption
{
#pragma warning disable CS8618 // Required by Entity Framework
    private QuestionOption()
    {

    }

    public QuestionOption(Guid id, string text, bool isFlagged)
    {
        Id = id;
        Text = text;
        IsFlagged = isFlagged;
    }

    public Guid Id { get; private set; }
    public string Text { get; private set; }
    public bool IsFlagged { get; private set; }
}
