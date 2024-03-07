namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class TextInputQuestionRequest : BaseQuestionRequest
{
    public TranslatedString? InputPlaceholder { get; set; }
}
