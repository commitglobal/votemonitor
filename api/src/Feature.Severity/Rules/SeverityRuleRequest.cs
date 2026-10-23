using Feature.Review.Rules;

namespace Feature.Severity.Rules;

/// <summary>A severity rule, as sent when creating or updating one.</summary>
public class SeverityRuleRequest : SaveRuleRequest
{
    /// <summary>"Set severity to" this level.</summary>
    public SeverityLevel Severity { get; set; } = null!;
}

public abstract class SeverityRuleRequestValidator<T> : SaveRuleRequestValidator<T> where T : SeverityRuleRequest
{
    protected SeverityRuleRequestValidator() : base(AutomationRuleKind.Severity)
    {
        RuleFor(x => x.Severity)
            .NotNull()
            .NotEqual(SeverityLevel.None)
            .WithMessage("A rule must set a severity above None");
    }
}
