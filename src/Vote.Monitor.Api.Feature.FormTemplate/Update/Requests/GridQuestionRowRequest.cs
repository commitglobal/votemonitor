namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class GridQuestionRowRequest
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public TranslatedString Text { get; set; }
    public TranslatedString? Helptext { get; set; }
}
