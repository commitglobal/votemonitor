namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public class DisplayLogic
{
    public Guid ParentQuestionId { get; }
    public DisplayLogicCondition Condition { get; }
    public string Value { get; }

    internal DisplayLogic(Guid parentQuestionId, DisplayLogicCondition condition, string value)
    {
        ParentQuestionId = parentQuestionId;
        Condition = condition;
        Value = value;
    }

    public static DisplayLogic Create(Guid parentQuestionId, DisplayLogicCondition condition, string value) =>
        new(parentQuestionId, condition, value);
}
