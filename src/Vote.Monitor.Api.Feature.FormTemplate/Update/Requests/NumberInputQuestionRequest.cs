namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class NumberInputQuestionRequest : BaseQuestionRequest
{
    public string Code { get; set; }
    public TranslatedString? InputPlaceholder { get; set; }
}
