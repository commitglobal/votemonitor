namespace SubmissionsFaker.Clients.Models.Questions;

public class TextQuestionRequest : BaseQuestionRequest
{
    public TranslatedString? InputPlaceholder { get; set; }
}
