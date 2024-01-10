namespace Vote.Monitor.Api.Feature.Forms.Update.Models;

public class QuestionOptionRequest
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsFlagged { get; set; }
}
