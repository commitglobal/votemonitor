namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>What an automation rule does: add reviewers, raise a record's severity, set its follow-up status or add labels.</summary>
[JsonConverter(typeof(SmartEnumValueConverter<AutomationRuleKind, string>))]
public sealed class AutomationRuleKind : SmartEnum<AutomationRuleKind, string>
{
    public static readonly AutomationRuleKind Reviewer = new(nameof(Reviewer), nameof(Reviewer));
    public static readonly AutomationRuleKind Severity = new(nameof(Severity), nameof(Severity));
    public static readonly AutomationRuleKind FollowUp = new(nameof(FollowUp), nameof(FollowUp));
    public static readonly AutomationRuleKind Labels = new(nameof(Labels), nameof(Labels));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    public static bool TryParse(string value, out AutomationRuleKind result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private AutomationRuleKind(string name, string value) : base(name, value)
    {
    }
}
