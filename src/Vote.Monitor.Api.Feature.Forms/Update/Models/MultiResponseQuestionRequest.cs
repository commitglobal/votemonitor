namespace Vote.Monitor.Api.Feature.Forms.Update.Models;

public class MultiResponseQuestionRequest : BaseQuestionRequest
{
    public List<QuestionOptionRequest> Options { get; set; }
}
