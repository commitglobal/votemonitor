namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public class DisplayLogic
{
    public Guid ParentQuestionId { get; }

    [JsonConverter(typeof(SmartEnumNameConverter<DisplayLogicCondition, string>))]
    public DisplayLogicCondition Condition { get; }

    public string? Value { get; }

    public IReadOnlyList<string>? OptionIds { get; }

    [JsonConstructor]
    internal DisplayLogic(Guid parentQuestionId, DisplayLogicCondition condition, string? value,
        IReadOnlyList<string>? optionIds = null)
    {
        ParentQuestionId = parentQuestionId;
        Condition = condition;
        Value = value;
        OptionIds = optionIds;
    }

    public static DisplayLogic Create(Guid parentQuestionId, DisplayLogicCondition condition, string? value = null,
        IReadOnlyList<string>? optionIds = null) =>
        new(parentQuestionId, condition, value, optionIds);
}
