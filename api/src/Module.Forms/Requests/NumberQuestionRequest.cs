using Vote.Monitor.Core.Models;

namespace Module.Forms.Requests;

public class NumberQuestionRequest : BaseQuestionRequest
{
    public TranslatedString? InputPlaceholder { get; set; }
}
