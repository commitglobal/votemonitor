using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Form.Module.Requests;

public class DisplayLogicRequest
{
    public Guid ParentQuestionId { get; set; }
    public DisplayLogicCondition Condition { get; set; }
    public string Value { get; set; }
}
