namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class TextInputQuestionRequest : BaseQuestionRequest
{
    public string Code { get; set; }
    public TranslatedString? InputPlaceholder { get; set; }
}
