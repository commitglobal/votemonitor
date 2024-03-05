namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class SelectOptionRequest
{
    public Guid Id { get; set; }
    public TranslatedString Text { get; set; }
    public bool IsFlagged { get; set; }
    public bool IsFreeText { get; set; }
}
