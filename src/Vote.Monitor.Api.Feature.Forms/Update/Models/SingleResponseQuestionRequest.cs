namespace Vote.Monitor.Api.Feature.Forms.Update.Models;

public class SingleResponseQuestionRequest : BaseQuestionRequest
{
    public List<QuestionOptionRequest> Options { get; set; }
}
