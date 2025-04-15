using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Models;

public class DisplayLogicModel
{
    public Guid ParentQuestionId { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<DisplayLogicCondition, string>))]
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
