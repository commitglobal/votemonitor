namespace Vote.Monitor.Api.Feature.Forms.Update.Models;

public class OpenQuestionRequest : BaseQuestionRequest
{
    public string Placeholder { get; set; }
    public bool LongAnswer { get; set; }
    public OpenQuestionType OpenQuestionType { get; set; }
}
