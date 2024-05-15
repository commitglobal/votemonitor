using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public class DisplayLogic
{
    public Guid ParentQuestionId { get; }

    [JsonConverter(typeof(SmartEnumNameConverter<DisplayLogicCondition, string>))]
    public DisplayLogicCondition Condition { get; }
    public string Value { get; }
   
    [JsonConstructor]
    internal DisplayLogic(Guid parentQuestionId, DisplayLogicCondition condition, string value)
    {
        ParentQuestionId = parentQuestionId;
        Condition = condition;
        Value = value;
    }

    public static DisplayLogic Create(Guid parentQuestionId, DisplayLogicCondition condition, string value) =>
        new(parentQuestionId, condition, value);
}
