namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

[JsonConverter(typeof(SmartEnumValueConverter<DisplayLogicCondition, string>))]

public class DisplayLogicCondition : SmartEnum<DisplayLogicCondition, string>
{
    public static readonly DisplayLogicCondition ValueEquals = new("Equals", "Equals");
    public static readonly DisplayLogicCondition NotEquals = new(nameof(NotEquals), nameof(NotEquals));
    public static readonly DisplayLogicCondition LessThan = new(nameof(LessThan), nameof(LessThan));
    public static readonly DisplayLogicCondition LessEqual = new(nameof(LessEqual), nameof(LessEqual));
    public static readonly DisplayLogicCondition GreaterThan = new(nameof(GreaterThan), nameof(GreaterThan));
    public static readonly DisplayLogicCondition GreaterEqual = new(nameof(GreaterEqual), nameof(GreaterEqual));
    public static readonly DisplayLogicCondition Includes = new(nameof(Includes), nameof(Includes));


    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="DisplayLogicCondition" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out DisplayLogicCondition result)
    {
        return TryFromValue(value, out result);
    }

    private DisplayLogicCondition(string name, string value) : base(name, value)
    {
    }
}
