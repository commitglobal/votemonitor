namespace Vote.Monitor.Api.Feature.Forms.Models;

public class QuestionOptionModel
{
    public Guid Id { get; private set; }
    public string Text { get; private set; }
    public bool IsFlagged { get; private set; }

    public QuestionOptionModel(Guid id, string text, bool isFlagged)
    {
        Id = id;
        Text = text;
        IsFlagged = isFlagged;
    }
}
