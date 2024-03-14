using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Form.Module.Requests;

public class TextQuestionRequest : BaseQuestionRequest
{
    public TranslatedString? InputPlaceholder { get; set; }
}
