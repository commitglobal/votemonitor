using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Form.Module.Requests;

public class NumberQuestionRequest : BaseQuestionRequest
{
    public TranslatedString? InputPlaceholder { get; set; }
}
