using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Form.Module.Models;

public class DisplayLogicModel
{
    public Guid ParentQuestionId { get; init; }
    public DisplayLogicCondition Condition { get; init; }
    public string Value { get; init; }

    public static DisplayLogicModel? FromEntity(DisplayLogic? entity) => entity == null
        ? null
        : new DisplayLogicModel
        {
            ParentQuestionId = entity.ParentQuestionId,
            Condition = entity.Condition,
            Value = entity.Value
        };
}
