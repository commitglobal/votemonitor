using Vote.Monitor.Core.Models;

namespace Module.Forms.Requests;

public class TextQuestionRequest : BaseQuestionRequest
{
    public TranslatedString? InputPlaceholder { get; set; }
}
